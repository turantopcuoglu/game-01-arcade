using UnityEngine;
using System;
using System.Collections.Generic;
using Project.Gameplay.Pooling;

namespace Project.Gameplay.Spawning
{
	/// <summary>
	/// Manages chunk-based spawning system.
	/// Coordinates obstacle and coin spawning based on patterns.
	/// </summary>
	public class ChunkManager : MonoBehaviour
	{
		public static ChunkManager Instance { get; private set; }

		[Header("Refs")]
		[SerializeField] private SimplePool obstaclePool;
		[SerializeField] private SimplePool coinPool;
		[SerializeField] private Transform obstacleParent;
		[SerializeField] private Transform coinParent;
		[SerializeField] private DifficultyController difficulty;

		[Header("Chunk Settings")]
		[SerializeField] private float minChunkLength = 25f;
		[SerializeField] private float maxChunkLength = 35f;
		[SerializeField] private float spawnZ = 30f;
		[SerializeField] private float despawnZ = -12f;

		[Header("Spawn Heights")]
		[SerializeField] private float obstacleY = 0.75f;
		[SerializeField] private float coinY = 1.2f;

		[Header("Rush Segment")]
		[SerializeField] private int chunksBeforeRush = 4;      // Rush every N chunks
		[SerializeField] private float rushSpeedMultiplier = 1.25f;
		[SerializeField] private float rushDuration = 5f;

		[Header("Lane Settings")]
		[SerializeField] private float minX = -2.2f;
		[SerializeField] private float maxX = 2.2f;

		// Events
		public event Action<SpawnPattern> OnPatternStarted;
		public event Action OnRushStarted;
		public event Action OnRushEnded;
		public event Action<int> OnChunkCompleted;

		// State
		private bool _running;
		private float _distanceTraveled;
		private float _chunkEndDistance;
		private int _chunksCompleted;
		private SpawnPattern _currentPattern;
		private List<SpawnedObject> _activeObjects = new List<SpawnedObject>();

		// Rush state
		private bool _inRush;
		private float _rushTimer;
		private float _preRushSpeed;

		// Track what's been spawned in current chunk
		private int _nextObstacleIndex;
		private int _nextCoinIndex;
		private float _chunkStartZ;

		private struct SpawnedObject
		{
			public GameObject obj;
			public float spawnZ;
		}

		private void Awake()
		{
			if (Instance != null) { Destroy(gameObject); return; }
			Instance = this;
		}

		public void StartSpawning()
		{
			_running = true;
			_distanceTraveled = 0f;
			_chunksCompleted = 0;
			_inRush = false;
			_activeObjects.Clear();

			// Start first chunk
			StartNewChunk();
		}

		public void StopSpawning()
		{
			_running = false;

			if (_inRush)
			{
				EndRush();
			}
		}

		public void ResetManager()
		{
			_running = false;
			_distanceTraveled = 0f;
			_chunksCompleted = 0;
			_inRush = false;
			_rushTimer = 0f;
			_currentPattern = null;
			_activeObjects.Clear();
		}

		private void Update()
		{
			if (!_running) return;

			float speed = GetCurrentSpeed();
			float dt = Time.deltaTime;
			float distanceThisFrame = speed * dt;

			_distanceTraveled += distanceThisFrame;

			// Update rush timer
			if (_inRush)
			{
				_rushTimer -= dt;
				if (_rushTimer <= 0f)
				{
					EndRush();
				}
			}

			// Spawn objects as we progress through chunk
			SpawnPendingObjects();

			// Move and cleanup active objects
			UpdateActiveObjects(distanceThisFrame);

			// Check if chunk is complete
			if (_distanceTraveled >= _chunkEndDistance)
			{
				CompleteChunk();
			}
		}

		private void StartNewChunk()
		{
			float chunkLength = UnityEngine.Random.Range(minChunkLength, maxChunkLength);
			float currentDifficulty = difficulty != null ? difficulty.CurrentSpeed / 11f : 0.5f;

			// Check for rush segment
			if (!_inRush && _chunksCompleted > 0 && _chunksCompleted % chunksBeforeRush == 0)
			{
				StartRush();
				_currentPattern = PatternGenerator.Generate(PatternType.StraightRush, chunkLength, currentDifficulty);
			}
			else
			{
				PatternType type = PatternGenerator.SelectRandomPattern(currentDifficulty, !_inRush);
				_currentPattern = PatternGenerator.Generate(type, chunkLength, currentDifficulty);
			}

			_chunkStartZ = spawnZ;
			_chunkEndDistance = _distanceTraveled + chunkLength;
			_nextObstacleIndex = 0;
			_nextCoinIndex = 0;

			OnPatternStarted?.Invoke(_currentPattern);
		}

		private void SpawnPendingObjects()
		{
			if (_currentPattern == null) return;

			float progressInChunk = _distanceTraveled - (_chunkEndDistance - _currentPattern.Length);

			// Spawn obstacles
			while (_nextObstacleIndex < _currentPattern.Obstacles.Count)
			{
				var obstacle = _currentPattern.Obstacles[_nextObstacleIndex];

				// Spawn when we're within spawn distance
				if (obstacle.zOffset <= progressInChunk + spawnZ)
				{
					SpawnObstacle(obstacle, spawnZ - progressInChunk + obstacle.zOffset);
					_nextObstacleIndex++;
				}
				else
				{
					break;
				}
			}

			// Spawn coins
			while (_nextCoinIndex < _currentPattern.Coins.Count)
			{
				var coin = _currentPattern.Coins[_nextCoinIndex];

				if (coin.zOffset <= progressInChunk + spawnZ)
				{
					SpawnCoin(coin, spawnZ - progressInChunk + coin.zOffset);
					_nextCoinIndex++;
				}
				else
				{
					break;
				}
			}
		}

		private void SpawnObstacle(ObstacleData data, float worldZ)
		{
			if (obstaclePool == null) return;

			float speed = GetCurrentSpeed();

			switch (data.type)
			{
				case ObstacleType.Static:
					SpawnStaticObstacle(data.xPosition, worldZ, speed);
					break;

				case ObstacleType.Sweeper:
					SpawnSweeperObstacle(data.xPosition, worldZ, speed, data.sweeperSpeed, data.sweeperRange);
					break;

				case ObstacleType.Gate:
					SpawnGateObstacle(data.xPosition, worldZ, speed, data.gateGapWidth);
					break;
			}
		}

		private void SpawnStaticObstacle(float x, float z, float speed)
		{
			var go = obstaclePool.Get();
			go.transform.SetParent(obstacleParent, false);
			go.transform.position = new Vector3(x, obstacleY, z);
			go.transform.rotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;

			var mover = go.GetComponent<ObstacleMover>();
			if (mover != null)
			{
				mover.Init(obstaclePool, speed, despawnZ);
				mover.SetMovementType(ObstacleMovementType.Static);
			}

			_activeObjects.Add(new SpawnedObject { obj = go, spawnZ = z });
		}

		private void SpawnSweeperObstacle(float x, float z, float speed, float sweeperSpeed, float sweeperRange)
		{
			var go = obstaclePool.Get();
			go.transform.SetParent(obstacleParent, false);
			go.transform.position = new Vector3(x, obstacleY, z);
			go.transform.rotation = Quaternion.identity;

			// Make it wider for sweeper effect
			go.transform.localScale = new Vector3(2f, 1f, 0.5f);

			var mover = go.GetComponent<ObstacleMover>();
			if (mover != null)
			{
				mover.Init(obstaclePool, speed, despawnZ);
				mover.SetMovementType(ObstacleMovementType.Sweeper, sweeperSpeed, sweeperRange, minX, maxX);
			}

			_activeObjects.Add(new SpawnedObject { obj = go, spawnZ = z });
		}

		private void SpawnGateObstacle(float gapCenter, float z, float speed, float gapWidth)
		{
			// Spawn left pillar
			float leftX = gapCenter - gapWidth / 2f - 1f;  // 1 unit is half obstacle width
			if (leftX > minX)
			{
				var leftGo = obstaclePool.Get();
				leftGo.transform.SetParent(obstacleParent, false);

				// Scale to fill from minX to gap
				float leftWidth = (gapCenter - gapWidth / 2f) - minX;
				leftGo.transform.localScale = new Vector3(leftWidth, 1f, 1f);
				leftGo.transform.position = new Vector3(minX + leftWidth / 2f, obstacleY, z);
				leftGo.transform.rotation = Quaternion.identity;

				var mover = leftGo.GetComponent<ObstacleMover>();
				if (mover != null)
				{
					mover.Init(obstaclePool, speed, despawnZ);
					mover.SetMovementType(ObstacleMovementType.Static);
				}

				_activeObjects.Add(new SpawnedObject { obj = leftGo, spawnZ = z });
			}

			// Spawn right pillar
			float rightX = gapCenter + gapWidth / 2f + 1f;
			if (rightX < maxX)
			{
				var rightGo = obstaclePool.Get();
				rightGo.transform.SetParent(obstacleParent, false);

				// Scale to fill from gap to maxX
				float rightWidth = maxX - (gapCenter + gapWidth / 2f);
				rightGo.transform.localScale = new Vector3(rightWidth, 1f, 1f);
				rightGo.transform.position = new Vector3(maxX - rightWidth / 2f, obstacleY, z);
				rightGo.transform.rotation = Quaternion.identity;

				var mover = rightGo.GetComponent<ObstacleMover>();
				if (mover != null)
				{
					mover.Init(obstaclePool, speed, despawnZ);
					mover.SetMovementType(ObstacleMovementType.Static);
				}

				_activeObjects.Add(new SpawnedObject { obj = rightGo, spawnZ = z });
			}
		}

		private void SpawnCoin(CoinData data, float worldZ)
		{
			if (coinPool == null) return;

			var go = coinPool.Get();
			go.transform.SetParent(coinParent, false);
			go.transform.position = new Vector3(data.xPosition, coinY, worldZ);
			go.transform.rotation = Quaternion.identity;

			float speed = GetCurrentSpeed();

			var mover = go.GetComponent<CoinMover>();
			if (mover != null) mover.Init(coinPool, speed, despawnZ);

			var pickup = go.GetComponent<CoinPickup>();
			if (pickup != null) pickup.Init(coinPool);

			_activeObjects.Add(new SpawnedObject { obj = go, spawnZ = worldZ });
		}

		private void UpdateActiveObjects(float distanceThisFrame)
		{
			// Remove despawned objects from tracking
			_activeObjects.RemoveAll(so => so.obj == null || !so.obj.activeInHierarchy);
		}

		private void CompleteChunk()
		{
			_chunksCompleted++;
			OnChunkCompleted?.Invoke(_chunksCompleted);

			// Start next chunk
			StartNewChunk();
		}

		private void StartRush()
		{
			_inRush = true;
			_rushTimer = rushDuration;
			_preRushSpeed = difficulty != null ? difficulty.CurrentSpeed : 6f;

			OnRushStarted?.Invoke();
		}

		private void EndRush()
		{
			_inRush = false;
			_rushTimer = 0f;

			OnRushEnded?.Invoke();
		}

		private float GetCurrentSpeed()
		{
			float baseSpeed = difficulty != null ? difficulty.CurrentSpeed : 6f;

			if (_inRush)
			{
				return baseSpeed * rushSpeedMultiplier;
			}

			return baseSpeed;
		}

		// Public getters
		public bool IsInRush => _inRush;
		public float RushTimeRemaining => _rushTimer;
		public int ChunksCompleted => _chunksCompleted;
		public PatternType CurrentPatternType => _currentPattern?.Type ?? PatternType.Empty;
		public float CurrentSpeed => GetCurrentSpeed();
	}
}
