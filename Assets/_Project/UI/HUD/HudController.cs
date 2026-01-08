using UnityEngine;
using TMPro;
using Project.Core;
using Project.Gameplay;

namespace Project.UI
{
	public class HUDController : MonoBehaviour
	{
		[Header("Visibility")]
		[SerializeField] private GameObject root; // HUDRoot

		[Header("Bindings")]
		[SerializeField] private TMP_Text scoreText;
		[SerializeField] private TMP_Text coinText;

		private void Awake()
		{
			if (root == null) root = gameObject;
		}

		private void Update()
		{
			var gm = GameManager.Instance;
			if (gm == null) return;

			bool show = gm.CurrentState == GameStateId.Gameplay || gm.CurrentState == GameStateId.Pause;

			// KENDÝNÝ DEÐÝL root'u toggle et (root == gameObject ise de sorun yok, çünkü Update sadece show=true iken çaðrýlýr)
			if (root != null && root.activeSelf != show)
				root.SetActive(show);

			// Eðer root kapandýysa bu frame sonrasý Update duracak; bu beklenen.
			// Ancak show=true olduðunda tekrar açýlacaðý için sorun yok.

			var session = RunSession.Instance;
			if (session == null) return;

			if (scoreText != null) scoreText.text = $"Score: {session.Score}";
			if (coinText != null) coinText.text = $"Coins: {session.Coins}";
		}
	}
}
