using UnityEngine;
using UnityEngine.EventSystems;
using Project.Core;
using Project.Systems.Input;

namespace Project.UI
{
	public class TapToStart : MonoBehaviour
	{
		private void OnEnable()
		{
			InputRouter.Instance.OnTapScreen += OnTap;
		}

		private void OnDisable()
		{
			if (InputRouter.Instance != null)
				InputRouter.Instance.OnTapScreen -= OnTap;
		}

		private void OnTap(Vector2 screenPos)
		{
			// 1) UI üzerindeyse ignore (butonlara basýnca oyunu baþlatma!)
			if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
				return;

			// 2) Sadece Menu state'te start
			if (GameManager.Instance.CurrentState != GameStateId.Menu)
				return;

			GameManager.Instance.StartGame();
		}
	}
}
