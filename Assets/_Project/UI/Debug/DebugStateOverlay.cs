using TMPro;
using UnityEngine;

namespace Project.UI
{
	public class DebugStateOverlay : MonoBehaviour
	{
		[SerializeField] private TMP_Text text;

		private void Update()
		{
			if (text == null) return;
			var gm = Project.Core.GameManager.Instance;
			text.text = gm == null ? "GM: null" : $"State: {gm.CurrentState}";
		}
	}
}
