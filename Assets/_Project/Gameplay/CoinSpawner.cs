using UnityEngine;
using Project.Gameplay.Pooling;

namespace Project.Gameplay
{
	/// <summary>
	/// Spawns coin trails independently from obstacles.
	/// Coins appear in groups (trails) in safe lanes where obstacles aren't.
	/// This is the standard pattern for endless runner games.
	/// </summary>
	public class CoinSpawner : MonoBehaviour
	{
		[Header("Refs")]
		[SerializeField] private SimplePool coinPool;
		[SerializeField] private Transform coinParent;
		[SerializeField] private ObstacleSpawner obstacleSpawner;
		[SerializeField] private DifficultyController difficulty;

		[Header("Spawn Settings")]
		[SerializeField] private float spawnZ = 30f;           // Same as obstacle spawn distance
		[SerializeField] private float despawnZ = -12f;
		[SerializeField] private float coinY = 1.2f;

		[Header("Trail Settings")]
		[SerializeField] private int minCoinsPerTrail = 4;
		[SerializeField] private int maxCoinsPerTrail = 8;
		[SerializeField] private float coinSpacing = 1.5f;     // Distance between coins in a trail

		[Header("Timing")]
		[SerializeField] private float minSpawnInterval = 2.0f;  // Minimum time between trails
		[SerializeField] private float maxSpawnInterval = 4.0f;  // Maximum time between trails

		[Header("Lane System")]
		[SerializeField] private float laneWidth = 1.5f;       // Width of each lane
		[SerializeField] private float minX = -2.2f;
		[SerializeField] private float maxX = 2.2f;

		private float _timer;
		private float _currentInterval;
		private bool _running;

		public void StartSpawn()
		{
			_running = true;
			_currentInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
			_timer = 0f;
		}

		public void StopSpawn()
		{
			_running = false;
		}

		public void ResetSpawner()
		{
			_timer = 0f;
			_currentInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
		}

		private void Update()
		{
			if (!_running) return;

			_timer += Time.deltaTime;
			if (_timer >= _currentInterval)
			{
				_timer = 0f;
				_currentInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
				SpawnCoinTrail();
			}
		}

		private void SpawnCoinTrail()
		{
			if (coinPool == null || coinParent == null) return;

			// Find a safe lane (away from last obstacle)
			float safeX = GetSafeLanePosition();

			// Determine trail length
			int coinCount = Random.Range(minCoinsPerTrail, maxCoinsPerTrail + 1);

			// Get current speed for coin movement
			float speed = difficulty != null ? difficulty.CurrentSpeed : 6f;

			// Spawn coins in a trail formation
			float startZ = spawnZ;
			for (int i = 0; i < coinCount; i++)
			{
				SpawnCoin(safeX, startZ + (i * coinSpacing), speed);
			}
		}

		/// <summary>
		/// Finds a lane position that's safe from the last spawned obstacle.
		/// Runner games typically place coins where obstacles aren't.
		/// </summary>
		private float GetSafeLanePosition()
		{
			float obstacleX = 0f;

			// Get last obstacle position if available
			if (obstacleSpawner != null)
			{
				obstacleX = obstacleSpawner.LastObstacleX;
			}

			// Calculate available lanes
			float totalWidth = maxX - minX;
			int laneCount = Mathf.Max(3, Mathf.FloorToInt(totalWidth / laneWidth));
			float actualLaneWidth = totalWidth / laneCount;

			// Find which lane the obstacle is in
			int obstacleLane = Mathf.FloorToInt((obstacleX - minX) / actualLaneWidth);
			obstacleLane = Mathf.Clamp(obstacleLane, 0, laneCount - 1);

			// Pick a different lane for coins
			int safeLane;
			if (laneCount <= 1)
			{
				safeLane = 0;
			}
			else
			{
				// Prefer lanes that are not adjacent to obstacle lane
				int attempts = 0;
				do
				{
					safeLane = Random.Range(0, laneCount);
					attempts++;
				}
				while (Mathf.Abs(safeLane - obstacleLane) < 1 && attempts < 10);
			}

			// Convert lane index to X position (center of lane)
			float laneX = minX + (safeLane * actualLaneWidth) + (actualLaneWidth * 0.5f);
			return laneX;
		}

		private void SpawnCoin(float x, float z, float speed)
		{
			var go = coinPool.Get();
			go.transform.SetParent(coinParent, false);
			go.transform.position = new Vector3(x, coinY, z);
			go.transform.rotation = Quaternion.identity;

			var mover = go.GetComponent<CoinMover>();
			if (mover != null) mover.Init(coinPool, speed, despawnZ);

			var pickup = go.GetComponent<CoinPickup>();
			if (pickup != null) pickup.Init(coinPool);
		}
	}
}
