using UnityEngine;
using Project.Gameplay.Pooling;

namespace Project.Gameplay
{
	public class ObstacleMover : MonoBehaviour
	{
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
			transform.position += Vector3.back * _speed * Time.deltaTime;

			if (transform.position.z <= _despawnZ)
			{
				if (_pool != null) _pool.Return(gameObject);
				else gameObject.SetActive(false);
			}
		}
	}
}
