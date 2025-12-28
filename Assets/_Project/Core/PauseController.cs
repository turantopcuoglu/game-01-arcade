using UnityEngine;
using Project.Systems.Input;

namespace Project.Core
{
	public class PauseController : MonoBehaviour
	{
		private void OnEnable()
		{
			InputRouter.Instance.OnTwoFingerTap += HandlePause;
		}

		private void OnDisable()
		{
			if (InputRouter.Instance != null)
				InputRouter.Instance.OnTwoFingerTap -= HandlePause;
		}

		private void HandlePause()
		{
			// Sadece Menu/Gameplay/Pause arasýnda anlamlý
			if (GameManager.Instance == null) return;

			if (GameManager.Instance.CurrentState == GameStateId.Gameplay ||
				GameManager.Instance.CurrentState == GameStateId.Pause)
			{
				GameManager.Instance.TogglePause();
			}
		}
	}
}
