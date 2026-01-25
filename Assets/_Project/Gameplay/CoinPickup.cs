using UnityEngine;
using System.Collections;
using Project.Gameplay.Pooling;
using Project.Audio;
using Project.Systems.Haptics;

namespace Project.Gameplay
{
	public class CoinPickup : MonoBehaviour
	{
		[Header("Pickup VFX")]
		[SerializeField] private float popDuration = 0.1f;
		[SerializeField] private float popScale = 1.3f;

		private SimplePool _pool;
		private Vector3 _originalScale;
		private bool _collecting;

		private void Awake()
		{
			_originalScale = transform.localScale;
		}

		public void Init(SimplePool pool) => _pool = pool;

		private void OnEnable()
		{
			_collecting = false;
			transform.localScale = _originalScale;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (_collecting) return;
			if (other.GetComponentInParent<PlayerRunnerController>() == null) return;

			_collecting = true;

			// Add coin immediately
			RunSession.Instance.AddCoin(1);

			// Feedback
			Haptics.CoinPickup();
			AudioService.Instance?.PlayCoinPickup();

			// Pop VFX then return to pool
			StartCoroutine(PopAndReturn());
		}

		private IEnumerator PopAndReturn()
		{
			// Quick scale up
			float elapsed = 0f;
			float halfDuration = popDuration * 0.5f;

			while (elapsed < halfDuration)
			{
				float t = elapsed / halfDuration;
				transform.localScale = Vector3.Lerp(_originalScale, _originalScale * popScale, t);
				elapsed += Time.deltaTime;
				yield return null;
			}

			// Quick scale down to zero
			elapsed = 0f;
			Vector3 peakScale = _originalScale * popScale;

			while (elapsed < halfDuration)
			{
				float t = elapsed / halfDuration;
				transform.localScale = Vector3.Lerp(peakScale, Vector3.zero, t);
				elapsed += Time.deltaTime;
				yield return null;
			}

			transform.localScale = _originalScale;

			if (_pool != null) _pool.Return(gameObject);
			else gameObject.SetActive(false);
		}
	}
}