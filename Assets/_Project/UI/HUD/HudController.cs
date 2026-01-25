using UnityEngine;
using TMPro;
using Project.Core;
using Project.Gameplay;

namespace Project.UI
{
	/// <summary>
	/// Gameplay HUD - displays score and coins during gameplay.
	/// Panel visibility is managed by UIManager.
	/// </summary>
	public class HUDController : MonoBehaviour
	{
		[Header("Score Display")]
		[SerializeField] private TMP_Text scoreText;
		[SerializeField] private TMP_Text coinText;

		private void Update()
		{
			// Only update text during gameplay or pause (when HUD is visible)
			var gm = GameManager.Instance;
			if (gm == null) return;

			var state = gm.CurrentState;
			if (state != GameStateId.Gameplay && state != GameStateId.Pause && state != GameStateId.GameOver)
				return;

			var session = RunSession.Instance;
			if (session == null) return;

			if (scoreText != null)
				scoreText.text = $"Score: {session.Score}";

			if (coinText != null)
				coinText.text = $"Coins: {session.Coins}";
		}

		// -------- UI Button Callbacks --------

		public void OnPauseClicked()
		{
			Debug.Log("[UI] Pause clicked");
			UIManager.Instance?.OnPauseClicked();
		}

		public void OnSettingsClicked()
		{
			Debug.Log("[UI] Settings clicked from HUD");
			UIManager.Instance?.OnSettingsClicked();
		}
	}
}
