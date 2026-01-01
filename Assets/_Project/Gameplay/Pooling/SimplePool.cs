using System.Collections.Generic;
using UnityEngine;

namespace Project.Gameplay.Pooling
{
	public class SimplePool : MonoBehaviour
	{
		[SerializeField] private GameObject prefab;
		[SerializeField] private int prewarmCount = 30;

		private readonly Queue<GameObject> _pool = new();

		private void Awake()
		{
			if (prefab == null)
			{
				Debug.LogError("[SimplePool] Prefab is not assigned.");
				enabled = false;
				return;
			}

			Prewarm();
		}

		private void Prewarm()
		{
			for (int i = 0; i < prewarmCount; i++)
			{
				var go = CreateNew();
				Return(go);
			}
		}

		private GameObject CreateNew()
		{
			var go = Instantiate(prefab, transform);
			go.SetActive(false);
			return go;
		}

		public GameObject Get()
		{
			if (_pool.Count == 0)
				_pool.Enqueue(CreateNew());

			var go = _pool.Dequeue();
			go.SetActive(true);
			return go;
		}

		public void Return(GameObject go)
		{
			go.SetActive(false);
			go.transform.SetParent(transform, false);
			_pool.Enqueue(go);
		}
	}
}