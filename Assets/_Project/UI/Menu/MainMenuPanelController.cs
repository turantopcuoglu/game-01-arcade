using UnityEngine;
using TMPro;
using Project.Core;
using Project.Gameplay;

namespace Project.UI
{
	/// <summary>
	/// Main menu panel controller.
	/// Panel visibility is managed by UIManager.
	/// </summary>
	public class MainMenuPanelController : MonoBehaviour
	{
		[Header("Optional Best Score Display")]
		[SerializeField] private TMP_Text bestScoreText;
		[SerializeField] private GameObject bestScoreContainer;

		private void OnEnable()
		{
			RefreshBestScore();
		}

		private void RefreshBestScore()
		{
			var session = RunSession.Instance;
			if (session == null) return;

			bool hasBestScore = session.BestScore > 0;

			if (bestScoreContainer != null)
				bestScoreContainer.SetActive(hasBestScore);

			if (bestScoreText != null && hasBestScore)
				bestScoreText.text = $"Best: {session.BestScore}";
		}

		// -------- UI Button Callbacks --------

		public void OnStartClicked()
		{
			Debug.Log("[UI] Start clicked");
			UIManager.Instance?.OnStartGameClicked();
		}

		public void OnSettingsClicked()
		{
			Debug.Log("[UI] Settings clicked from Menu");
			UIManager.Instance?.OnSettingsClicked();
		}
	}
}
