using UnityEngine;
using Project.Core;
using Project.Audio;
using Project.Systems.Haptics;

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

			if (other.GetComponent<ObstacleMover>() == null) return;

			_dead = true;

			// Collision feedback
			CameraShake.Instance?.ShakeMedium();
			Haptics.Impact();
			AudioService.Instance?.PlayCollision();

			GameManager.Instance.GameOver();
		}
	}
}
