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
		private const string BEST_COINS_KEY = "BestCoins";

		public int Coins { get; private set; }
		public int Score { get; private set; }
		public int BestScore { get; private set; }
		public int BestCoins { get; private set; }
		public int TotalCoinsCollected { get; private set; }  // Raw count (ignores multiplier)

		private float _scoreAcc;

		private void Awake()
		{
			if (Instance != null) { Destroy(gameObject); return; }
			Instance = this;
			DontDestroyOnLoad(gameObject);

			// Load best records from PlayerPrefs
			BestScore = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
			BestCoins = PlayerPrefs.GetInt(BEST_COINS_KEY, 0);
		}

		/// <summary>
		/// Resets current run data. Call this when starting a new game.
		/// Best score is preserved.
		/// </summary>
		public void ResetSession()
		{
			Coins = 0;
			Score = 0;
			TotalCoinsCollected = 0;
			_scoreAcc = 0f;

			// Reset combo system if available
			if (ComboSystem.Instance != null)
				ComboSystem.Instance.ResetCombo();
		}

		/// <summary>
		/// Add coins with combo multiplier applied.
		/// Returns the actual coin value added.
		/// </summary>
		public int AddCoin()
		{
			TotalCoinsCollected++;

			int coinValue = 1;
			if (ComboSystem.Instance != null)
			{
				coinValue = ComboSystem.Instance.RegisterCoinPickup();
			}

			Coins += coinValue;
			return coinValue;
		}

		/// <summary>
		/// Add coins directly without combo (for bonuses, etc).
		/// </summary>
		public void AddCoinsRaw(int amount)
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
		/// Call this when game ends to update best records if needed.
		/// </summary>
		public void TryUpdateBestScore()
		{
			bool updated = false;

			if (Score > BestScore)
			{
				BestScore = Score;
				PlayerPrefs.SetInt(BEST_SCORE_KEY, BestScore);
				updated = true;
				Debug.Log($"[RunSession] New best score: {BestScore}");
			}

			if (Coins > BestCoins)
			{
				BestCoins = Coins;
				PlayerPrefs.SetInt(BEST_COINS_KEY, BestCoins);
				updated = true;
				Debug.Log($"[RunSession] New best coins: {BestCoins}");
			}

			if (updated)
				PlayerPrefs.Save();
		}
	}
}