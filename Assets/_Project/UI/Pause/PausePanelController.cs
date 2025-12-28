using UnityEngine;
using Project.Core;

namespace Project.UI
{
	public class PausePanelController : MonoBehaviour
	{
		[SerializeField] private GameObject panelRoot;

		private void Update()
		{
			var gm = GameManager.Instance;
			if (gm == null || panelRoot == null) return;

			panelRoot.SetActive(gm.CurrentState == GameStateId.Pause);
		}
	}
}
