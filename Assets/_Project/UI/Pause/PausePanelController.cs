using UnityEngine;
using Project.Core;

namespace Project.UI
{
	/// <summary>
	/// Pause panel controller with button callbacks.
	/// Panel visibility is managed by UIManager.
	/// </summary>
	public class PausePanelController : MonoBehaviour
	{
		// -------- UI Button Callbacks --------

		public void OnResumeClicked()
		{
			Debug.Log("[UI] Resume clicked");
			UIManager.Instance?.OnResumeClicked();
		}

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

		public void OnSettingsClicked()
		{
			Debug.Log("[UI] Settings clicked from Pause");
			UIManager.Instance?.OnSettingsClicked();
		}
	}
}
