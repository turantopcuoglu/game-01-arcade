using UnityEngine;
using Project.Core;
using Project.Gameplay;

namespace Project.UI
{
	public class GameOverPanelController : MonoBehaviour
	{
		[SerializeField] private GameObject panelRoot;

		private void Awake()
		{
			if (panelRoot == null) panelRoot = gameObject;
		}

		private void Update()
		{
			var gm = GameManager.Instance;
			if (gm == null) return;

			bool show = gm.CurrentState == GameStateId.GameOver;
			if (panelRoot.activeSelf != show) panelRoot.SetActive(show);
		}

		// UI Button hook
		public void OnRestartClicked()
		{
			Debug.Log("[UI] Restart clicked");
			GameManager.Instance.RestartRun();  // reset only
			//GameManager.Instance.StartGame();   // gameplay'e dön
		}

		public void OnMenuClicked()
		{
			Debug.Log("[UI] Menu clicked");
			GameManager.Instance.OpenMenu();    // menu'ye dön
		}

	}
}
