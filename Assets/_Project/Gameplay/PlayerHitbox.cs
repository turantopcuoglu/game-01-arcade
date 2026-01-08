using UnityEngine;
using Project.Core;

namespace Project.Gameplay
{
	public class PlayerHitbox : MonoBehaviour
	{
		private bool _dead;

		private void OnEnable()
		{
			_dead = false;
		}
		public void ResetHitbox()
		{
			Debug.Log("PlayerHitbox Reset");
			_dead = false;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (_dead) return;

			// obstacle mý? (ObstacleMover varlýðý yeterli)
			if (other.GetComponent<ObstacleMover>() == null) return;

			_dead = true;
			GameManager.Instance.GameOver();
		}
	}
}
