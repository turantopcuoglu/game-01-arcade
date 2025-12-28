using UnityEngine;
using Project.Core;
using Project.Systems.Input;
using Project.Systems.Haptics;
using Project.Audio;

namespace Project.UI
{
	public class TapToStart : MonoBehaviour
	{
		private void OnEnable() => InputRouter.Instance.OnTapScreen += HandleTap;

		private void OnDisable()
		{
			if (InputRouter.Instance != null)
				InputRouter.Instance.OnTapScreen -= HandleTap;
		}

		private void HandleTap(Vector2 screenPos)
		{
			// Click hissi paketi
			AudioService.Instance?.PlayClick(0.7f);
			Haptics.Click();

			GameManager.Instance.StartGame();
		}
	}
}
