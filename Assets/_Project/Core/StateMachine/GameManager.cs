using UnityEngine;
using Project.Gameplay.Spawning;

namespace Project.Core
{
	public sealed class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }

		private GameStateMachine _sm;

		public GameStateId CurrentState => _sm.CurrentId;

		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);

			_sm = new GameStateMachine();

			// Register states
			_sm.Register(new BootState(_sm));
			_sm.Register(new MenuState(_sm));
			_sm.Register(new GameplayState(_sm));
			_sm.Register(new PauseState(_sm));
			_sm.Register(new GameOverState(_sm));

			Debug.Log($"[GM] Awake InstanceID={GetInstanceID()}");

			_sm.ChangeState(GameStateId.Boot);
		}

		private void Update()
		{
			_sm.Tick(Time.deltaTime);
		}

		// -------- Public API (UI / Input) --------

		public void StartGame()
		{
			Time.timeScale = 1f;

			PrepareNewRun();

			// force re-enter: Gameplay'deyken bile Enter tekrar �al��s�n
			_sm.ChangeState(GameStateId.Gameplay, force: true);
		}


		public void OpenMenu()
		{
			Time.timeScale = 1f;

			// Leaving a run -> clean scene
			CleanupRun();

			_sm.ChangeState(GameStateId.Menu);
		}

		public void TogglePause()
		{
			if (CurrentState == GameStateId.Gameplay) Pause();
			else if (CurrentState == GameStateId.Pause) Resume();
		}

		public void Pause() => _sm.ChangeState(GameStateId.Pause);

		public void Resume()
		{
			Time.timeScale = 1f;
			_sm.ChangeState(GameStateId.Gameplay);
		}

		public void GameOver() => _sm.ChangeState(GameStateId.GameOver);

		// Optional alias (if some UI still calls RestartRun)
		public void RestartRun() => StartGame();

		// -------- Run Lifecycle (single source of truth) --------

		private void PrepareNewRun()
		{
			// 1) Return active pooled objects (fresh start)
			var cleaner = FindObjectOfType<Project.Gameplay.PooledObjectsCleaner>();
			cleaner?.ResetAll();

			// 2) Stop spawners before re-starting (safety)
			var chunkManager = FindObjectOfType<ChunkManager>();
			chunkManager?.StopSpawning();
			chunkManager?.ResetManager();

			// Legacy spawner support (fallback if ChunkManager not present)
			var obstacleSpawner = FindObjectOfType<Project.Gameplay.ObstacleSpawner>();
			obstacleSpawner?.StopSpawn();

			var coinSpawner = FindObjectOfType<Project.Gameplay.CoinSpawner>();
			coinSpawner?.ResetSpawner();

			// 3) Reset gameplay systems
			var diff = FindObjectOfType<Project.Gameplay.DifficultyController>();
			diff?.ResetDifficulty();

			// 4) Reset session (coins/score) - also resets combo
			Project.Gameplay.RunSession.Instance?.ResetSession();

			// 5) Reset player input state (critical)
			var playerCtrl = FindObjectOfType<Project.Gameplay.PlayerRunnerController>();
			playerCtrl?.ResetController();

			var hitbox = FindObjectOfType<Project.Gameplay.PlayerHitbox>();
			hitbox?.ResetHitbox();
		}

		private void CleanupRun()
		{
			// 1) Stop spawners (leaving gameplay)
			var chunkManager = FindObjectOfType<ChunkManager>();
			chunkManager?.StopSpawning();

			// Legacy spawner support
			var obstacleSpawner = FindObjectOfType<Project.Gameplay.ObstacleSpawner>();
			obstacleSpawner?.StopSpawn();

			var coinSpawner = FindObjectOfType<Project.Gameplay.CoinSpawner>();
			coinSpawner?.StopSpawn();

			// 2) Return active pooled objects (clean menu)
			var cleaner = FindObjectOfType<Project.Gameplay.PooledObjectsCleaner>();
			cleaner?.ResetAll();
		}

		// -------- Minimal State implementations --------

		private sealed class BootState : IGameState
		{
			private readonly GameStateMachine _sm;
			public GameStateId Id => GameStateId.Boot;

			public BootState(GameStateMachine sm) => _sm = sm;

			public void Enter()
			{
				Debug.Log("[State] Boot Enter");
				Time.timeScale = 1f;

				// Future: init services (save, analytics, ads). For now go to menu.
				_sm.ChangeState(GameStateId.Menu);
			}

			public void Exit() => Debug.Log("[State] Boot Exit");
			public void Tick(float dt) { }
		}

		private sealed class MenuState : IGameState
		{
			private readonly GameStateMachine _sm;
			public GameStateId Id => GameStateId.Menu;

			public MenuState(GameStateMachine sm) => _sm = sm;

			public void Enter()
			{
				Debug.Log("[State] Menu Enter");
				Time.timeScale = 1f;
			}

			public void Exit() => Debug.Log("[State] Menu Exit");
			public void Tick(float dt) { }
		}

		private sealed class GameplayState : IGameState
		{
			private readonly GameStateMachine _sm;
			public GameStateId Id => GameStateId.Gameplay;

			public GameplayState(GameStateMachine sm) => _sm = sm;

			public void Enter()
			{
				Debug.Log("[State] Gameplay Enter");
				Time.timeScale = 1f;

				// Prefer ChunkManager (new pattern-based system)
				var chunkManager = FindObjectOfType<ChunkManager>();
				if (chunkManager != null)
				{
					chunkManager.StartSpawning();
				}
				else
				{
					// Fallback to legacy spawners
					var obstacleSpawner = FindObjectOfType<Project.Gameplay.ObstacleSpawner>();
					obstacleSpawner?.StartSpawn();

					var coinSpawner = FindObjectOfType<Project.Gameplay.CoinSpawner>();
					coinSpawner?.StartSpawn();
				}
			}

			public void Exit()
			{
				Debug.Log("[State] Gameplay Exit");

				var chunkManager = FindObjectOfType<ChunkManager>();
				chunkManager?.StopSpawning();

				// Also stop legacy spawners (safety)
				var obstacleSpawner = FindObjectOfType<Project.Gameplay.ObstacleSpawner>();
				obstacleSpawner?.StopSpawn();

				var coinSpawner = FindObjectOfType<Project.Gameplay.CoinSpawner>();
				coinSpawner?.StopSpawn();
			}

			public void Tick(float dt) { }
		}

		private sealed class PauseState : IGameState
		{
			private readonly GameStateMachine _sm;
			public GameStateId Id => GameStateId.Pause;

			public PauseState(GameStateMachine sm) => _sm = sm;

			public void Enter()
			{
				Debug.Log("[State] Pause Enter");
				Time.timeScale = 0f;
			}

			public void Exit()
			{
				Debug.Log("[State] Pause Exit");
				Time.timeScale = 1f;
			}

			public void Tick(float dt) { }
		}

		private sealed class GameOverState : IGameState
		{
			private readonly GameStateMachine _sm;
			public GameStateId Id => GameStateId.GameOver;

			public GameOverState(GameStateMachine sm) => _sm = sm;

			public void Enter()
			{
				Debug.Log("[State] GameOver Enter");

				// Stop spawn loops immediately
				var chunkManager = FindObjectOfType<ChunkManager>();
				chunkManager?.StopSpawning();

				var obstacleSpawner = FindObjectOfType<Project.Gameplay.ObstacleSpawner>();
				obstacleSpawner?.StopSpawn();

				var coinSpawner = FindObjectOfType<Project.Gameplay.CoinSpawner>();
				coinSpawner?.StopSpawn();

				Time.timeScale = 0f;
			}

			public void Exit()
			{
				Debug.Log("[State] GameOver Exit");
				Time.timeScale = 1f;
			}

			public void Tick(float dt) { }
		}
	}
}
