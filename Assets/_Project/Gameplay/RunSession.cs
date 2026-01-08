using UnityEngine;

namespace Project.Gameplay
{
	public class RunSession : MonoBehaviour
	{
		public static RunSession Instance { get; private set; }

		public int Coins { get; private set; }
		public int Score { get; private set; }

		private float _scoreAcc;

		private void Awake()
		{
			if (Instance != null) { Destroy(gameObject); return; }
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		public void ResetSession()
		{
			Coins = 0;
			Score = 0;
			_scoreAcc = 0f;
		}

		public void AddCoin(int amount = 1)
		{
			Coins += amount;
		}

		public void TickScore(float deltaTime, float scorePerSecond)
		{
			_scoreAcc += deltaTime * scorePerSecond;
			int newScore = Mathf.FloorToInt(_scoreAcc);
			
			if (newScore > Score) Score = newScore;
		}
	}
}