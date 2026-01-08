using UnityEngine;
using Project.Gameplay.Pooling;

namespace Project.Gameplay
{
	public class PooledObjectsCleaner : MonoBehaviour
	{
		[SerializeField] private Transform obstaclesParent;
		[SerializeField] private SimplePool obstaclePool;

		[SerializeField] private Transform coinsParent;
		[SerializeField] private SimplePool coinPool;

		public void ResetAll()
		{
			Debug.Log("PooledObjectsCleaner ResetAll called");
			// Obstacles
			for (int i = obstaclesParent.childCount - 1; i >= 0; i--)
			{
				var go = obstaclesParent.GetChild(i).gameObject;
				if (go.activeSelf) obstaclePool.Return(go);
			}

			// Coins
			for (int i = coinsParent.childCount - 1; i >= 0; i--)
			{
				var go = coinsParent.GetChild(i).gameObject;
				if (go.activeSelf) coinPool.Return(go);
			}
		}
	}
}
