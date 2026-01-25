using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.Core;
using Project.Gameplay;
using Project.Gameplay.Spawning;

namespace Project.UI
{
	/// <summary>
	/// Gameplay HUD - displays score, coins, combo, and rush status during gameplay.
	/// Panel visibility is managed by UIManager.
	/// </summary>
	public class HUDController : MonoBehaviour
	{
		[Header("Score Display")]
		[SerializeField] private TMP_Text scoreText;
		[SerializeField] private TMP_Text coinText;

		[Header("Combo Display")]
		[SerializeField] private TMP_Text comboText;
		[SerializeField] private Image comboTimerBar;
		[SerializeField] private CanvasGroup comboGroup;

		[Header("Rush Display")]
		[SerializeField] private TMP_Text rushText;
		[SerializeField] private CanvasGroup rushGroup;

		[Header("Combo Colors")]
		[SerializeField] private Color comboTier0Color = Color.white;
		[SerializeField] private Color comboTier1Color = Color.yellow;
		[SerializeField] private Color comboTier2Color = new Color(1f, 0.5f, 0f);  // Orange
		[SerializeField] private Color comboTier3Color = Color.red;

		private int _lastCombo;
		private float _comboPopScale = 1f;

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

			// Score and coins
			if (scoreText != null)
				scoreText.text = $"Score: {session.Score}";

			if (coinText != null)
				coinText.text = $"Coins: {session.Coins}";

			// Combo display
			UpdateComboDisplay();

			// Rush display
			UpdateRushDisplay();
		}

		private void UpdateComboDisplay()
		{
			var combo = ComboSystem.Instance;
			if (combo == null)
			{
				if (comboGroup != null) comboGroup.alpha = 0f;
				return;
			}

			int currentCombo = combo.CurrentCombo;

			// Show/hide combo group
			if (comboGroup != null)
			{
				float targetAlpha = currentCombo > 0 ? 1f : 0f;
				comboGroup.alpha = Mathf.MoveTowards(comboGroup.alpha, targetAlpha, Time.deltaTime * 5f);
			}

			// Update combo text with pop effect on change
			if (comboText != null && currentCombo > 0)
			{
				if (currentCombo != _lastCombo)
				{
					_comboPopScale = 1.3f;
					_lastCombo = currentCombo;
				}

				_comboPopScale = Mathf.MoveTowards(_comboPopScale, 1f, Time.deltaTime * 3f);
				comboText.transform.localScale = Vector3.one * _comboPopScale;

				comboText.text = $"x{currentCombo}";

				// Color based on tier
				int tier = combo.GetComboTier();
				comboText.color = tier switch
				{
					0 => comboTier0Color,
					1 => comboTier1Color,
					2 => comboTier2Color,
					_ => comboTier3Color
				};
			}

			// Update combo timer bar
			if (comboTimerBar != null && currentCombo > 0)
			{
				comboTimerBar.fillAmount = combo.ComboTimeRatio;
			}
		}

		private void UpdateRushDisplay()
		{
			var chunk = ChunkManager.Instance;
			if (chunk == null || rushGroup == null)
			{
				if (rushGroup != null) rushGroup.alpha = 0f;
				return;
			}

			bool inRush = chunk.IsInRush;

			// Fade rush indicator
			float targetAlpha = inRush ? 1f : 0f;
			rushGroup.alpha = Mathf.MoveTowards(rushGroup.alpha, targetAlpha, Time.deltaTime * 5f);

			if (rushText != null && inRush)
			{
				float timeLeft = chunk.RushTimeRemaining;
				rushText.text = $"RUSH! {timeLeft:F1}s";
			}
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
