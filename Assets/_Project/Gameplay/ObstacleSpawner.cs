using UnityEngine;
using Project.Gameplay.Pooling;

namespace Project.Gameplay
{
	public class ObstacleSpawner : MonoBehaviour
	{
		[Header("Refs")]
		[SerializeField] private SimplePool obstaclePool;
		[SerializeField] private Transform obstacleParent;

		[Header("Spawn")]
		[SerializeField] private float spawnInterval = 1.2f;
		[SerializeField] private float spawnZ = 14f;
		[SerializeField] private float despawnZ = -12f;

		[Header("Lane Range")]
		[SerializeField] private float minX = -2.2f;
		[SerializeField] private float maxX = 2.2f;

		[Header("Tuning")]
		[SerializeField] private float obstacleSpeed = 6f;

		[SerializeField] private DifficultyController difficulty;
		[SerializeField] private CoinSpawner coinSpawner;



		private float _t;
		private bool _running;

		public void StartSpawn()
		{
			_t = 0f;
			_running = true;
		}

		public void StopSpawn()
		{
			_running = false;
		}

		private void Update()
		{
			if (!_running) return;

			float interval = difficulty != null ? difficulty.CurrentInterval : spawnInterval;
			_t += Time.deltaTime;
			if (_t >= interval)
			{
				_t = 0f;
				Spawn();
			}
		}

		private void Spawn()
		{
			if (obstaclePool == null || obstacleParent == null) return;

			var go = obstaclePool.Get();
			go.transform.SetParent(obstacleParent, false);

			float x = Random.Range(minX, maxX);
			go.transform.position = new Vector3(x, 0.75f, spawnZ);
			go.transform.rotation = Quaternion.identity;

			float speed = difficulty != null ? difficulty.CurrentSpeed : obstacleSpeed;
			var mover = go.GetComponent<ObstacleMover>();
			if (mover != null)
				mover.Init(obstaclePool, speed, despawnZ);

			if (coinSpawner != null)
				coinSpawner.TrySpawnForObstacle(x, spawnZ);

		}
	}
}