using UnityEngine;

namespace Project.Gameplay
{
	/// <summary>
	/// Tracks current run data (score, coins) and persistent best score.
	/// Reusable across games - just call ResetSession() on new run.
	/// </summary>
	public class RunSession : MonoBehaviour
	{
		public static RunSession Instance { get; private set; }

		private const string BEST_SCORE_KEY = "BestScore";

		public int Coins { get; private set; }
		public int Score { get; private set; }
		public int BestScore { get; private set; }

		private float _scoreAcc;

		private void Awake()
		{
			if (Instance != null) { Destroy(gameObject); return; }
			Instance = this;
			DontDestroyOnLoad(gameObject);

			// Load best score from PlayerPrefs
			BestScore = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
		}

		/// <summary>
		/// Resets current run data. Call this when starting a new game.
		/// Best score is preserved.
		/// </summary>
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

		/// <summary>
		/// Call this when game ends to update best score if needed.
		/// </summary>
		public void TryUpdateBestScore()
		{
			if (Score > BestScore)
			{
				BestScore = Score;
				PlayerPrefs.SetInt(BEST_SCORE_KEY, BestScore);
				PlayerPrefs.Save();
				Debug.Log($"[RunSession] New best score: {BestScore}");
			}
		}
	}
}