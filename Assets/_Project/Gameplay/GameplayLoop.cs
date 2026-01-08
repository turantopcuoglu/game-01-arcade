using UnityEngine;
using Project.Core;

namespace Project.Gameplay
{
	public class GameplayLoop : MonoBehaviour
	{
		[SerializeField] private DifficultyController difficulty;
		[SerializeField] private float scorePerSecond = 10f;

		private void Update()
		{
			if (GameManager.Instance.CurrentState != GameStateId.Gameplay) return;

			float dt = Time.deltaTime;
			difficulty?.Tick(dt);
			RunSession.Instance.TickScore(dt, scorePerSecond);
		}
	}
}
