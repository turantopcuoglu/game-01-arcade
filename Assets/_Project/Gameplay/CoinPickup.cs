using UnityEngine;
using Project.Gameplay.Pooling;

namespace Project.Gameplay
{
	public class CoinPickup : MonoBehaviour
	{
		private SimplePool _pool;

		public void Init(SimplePool pool) => _pool = pool;

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponent<PlayerRunnerController>() == null) return;

			RunSession.Instance.AddCoin(1);

			if (_pool != null) _pool.Return(gameObject);
			else gameObject.SetActive(false);
		}
	}
}