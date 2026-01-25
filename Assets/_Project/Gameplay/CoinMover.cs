using UnityEngine;
using Project.Gameplay.Pooling;

namespace Project.Gameplay
{
	public class CoinMover : MonoBehaviour
	{
		[Header("Spin Animation")]
		[SerializeField] private float spinSpeed = 180f;

		private SimplePool _pool;
		private float _speed;
		private float _despawnZ;

		public void Init(SimplePool pool, float speed, float despawnZ)
		{
			_pool = pool;
			_speed = speed;
			_despawnZ = despawnZ;
		}

		private void Update()
		{
			// Move backward
			transform.position += Vector3.back * _speed * Time.deltaTime;

			// Spin animation
			transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

			if (transform.position.z <= _despawnZ)
			{
				if (_pool != null) _pool.Return(gameObject);
				else gameObject.SetActive(false);
			}
		}
	}
}