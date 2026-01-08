using UnityEngine;
using Project.Gameplay.Pooling;

namespace Project.Gameplay
{
	public class CoinSpawner : MonoBehaviour
	{
		[SerializeField] private SimplePool coinPool;
		[SerializeField] private Transform coinParent;

		[SerializeField] private float coinY = 1.2f;
		[SerializeField] private float coinZOffset = 0.8f;

		[SerializeField] private float despawnZ = -12f;
		[SerializeField] private DifficultyController difficulty;

		private int _counter;

		public void ResetSpawner()
		{
			_counter = 0;
		}

		public void TrySpawnForObstacle(float obstacleX, float obstacleSpawnZ)
		{
			_counter++;
			if (_counter % 3 != 0) return;

			var go = coinPool.Get();
			go.transform.SetParent(coinParent, false);
			go.transform.position = new Vector3(obstacleX, coinY, obstacleSpawnZ + coinZOffset);
			go.transform.rotation = Quaternion.identity;

			float speed = difficulty != null ? difficulty.CurrentSpeed : 6f;
			var mover = go.GetComponent<CoinMover>();
			if (mover != null) mover.Init(coinPool, speed, despawnZ);

			var pickup = go.GetComponent<CoinPickup>();
			if (pickup != null) pickup.Init(coinPool);

		}
	}
}