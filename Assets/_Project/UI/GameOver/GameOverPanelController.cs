using UnityEngine;
using TMPro;
using Project.Core;
using Project.Gameplay;

namespace Project.UI
{
	/// <summary>
	/// GameOver panel controller - displays final score and best score.
	/// Panel visibility is managed by UIManager.
	/// </summary>
	public class GameOverPanelController : MonoBehaviour
	{
		[Header("Score Display")]
		[SerializeField] private TMP_Text scoreText;
		[SerializeField] private TMP_Text bestScoreText;
		[SerializeField] private TMP_Text gameOverText;

		[Header("Optional New Best Indicator")]
		[SerializeField] private GameObject newBestIndicator;

		private void OnEnable()
		{
			RefreshScoreDisplay();
		}

		/// <summary>
		/// Updates score display when panel becomes visible.
		/// </summary>
		private void RefreshScoreDisplay()
		{
			var session = RunSession.Instance;
			if (session == null) return;

			// Update best score before displaying
			int scoreBefore = session.BestScore;
			session.TryUpdateBestScore();
			bool isNewBest = session.Score > scoreBefore && scoreBefore > 0;

			if (scoreText != null)
				scoreText.text = $"Score: {session.Score}";

			if (bestScoreText != null)
				bestScoreText.text = $"Best: {session.BestScore}";

			if (newBestIndicator != null)
				newBestIndicator.SetActive(isNewBest);
		}

		// -------- UI Button Callbacks --------

		public void OnRestartClicked()
		{
			Debug.Log("[UI] Restart clicked");
			UIManager.Instance?.OnRestartClicked();
		}

		public void OnMenuClicked()
		{
			Debug.Log("[UI] Menu clicked");
			UIManager.Instance?.OnMainMenuClicked();
		}
	}
}
