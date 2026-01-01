using UnityEngine;

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

			// Register states (bugün inline basit tutuyoruz)
			_sm.Register(new BootState(_sm));
			_sm.Register(new MenuState(_sm));
			_sm.Register(new GameplayState(_sm));
			_sm.Register(new PauseState(_sm));
			_sm.Register(new GameOverState(_sm));

			_sm.ChangeState(GameStateId.Boot);
		}
		public void TogglePause()
		{
			if (CurrentState == GameStateId.Gameplay) Pause();
			else if (CurrentState == GameStateId.Pause) Resume();
		}

		private void Update()
		{
			// KURAL: Tick sadece aktif state’e gider.
			_sm.Tick(Time.deltaTime);
		}

		// UI / Input buradan state deðiþtirecek
		public void StartGame() => _sm.ChangeState(GameStateId.Gameplay);
		public void OpenMenu() => _sm.ChangeState(GameStateId.Menu);
		public void Pause() => _sm.ChangeState(GameStateId.Pause);
		public void GameOver() => _sm.ChangeState(GameStateId.GameOver);
		public void Resume() => _sm.ChangeState(GameStateId.Gameplay);

		// --- Minimal State implementations (Gün 2 için tek dosyada da olur ama biz düzgün gidiyoruz) ---
		private sealed class BootState : IGameState
		{
			private readonly GameStateMachine _sm;
			public GameStateId Id => GameStateId.Boot;

			public BootState(GameStateMachine sm) => _sm = sm;

			public void Enter()
			{
				Debug.Log("[State] Boot Enter");
				// Ýleride: service init, save init, ads/analytics stub vs.
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

			public void Enter() => Debug.Log("[State] Menu Enter");
			public void Exit() => Debug.Log("[State] Menu Exit");

			public void Tick(float dt)
			{
				// Þimdilik boþ. UI butonu StartGame() çaðýracak.
			}
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
				var spawner = GameObject.FindObjectOfType<Project.Gameplay.ObstacleSpawner>();
				if (spawner != null) spawner.StartSpawn();

			}

			public void Exit() 
			{
				Debug.Log("[State] Gameplay Exit");
				var spawner = GameObject.FindObjectOfType<Project.Gameplay.ObstacleSpawner>();
				if (spawner != null) spawner.StopSpawn();

			}

			public void Tick(float dt)
			{
				// Bugün test için basit log:
				// Debug.Log("[State] Gameplay Tick"); // spam yapma, gerekirse aç
			}
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
				var spawner = GameObject.FindObjectOfType<Project.Gameplay.ObstacleSpawner>();
				if (spawner != null) spawner.StopSpawn();

				Debug.Log("[State] GameOver Enter");
				//Time.timeScale = 0f;
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
