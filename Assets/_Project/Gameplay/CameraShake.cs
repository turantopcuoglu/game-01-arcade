using UnityEngine;
using System.Collections;

namespace Project.Gameplay
{
	public class CameraShake : MonoBehaviour
	{
		public static CameraShake Instance { get; private set; }

		[Header("Settings")]
		[SerializeField] private float defaultDuration = 0.15f;
		[SerializeField] private float defaultMagnitude = 0.08f;

		private Vector3 _originalPosition;
		private Coroutine _shakeCoroutine;

		private void Awake()
		{
			if (Instance != null) { Destroy(this); return; }
			Instance = this;
			_originalPosition = transform.localPosition;
		}

		public void Shake(float duration = -1f, float magnitude = -1f)
		{
			if (duration < 0) duration = defaultDuration;
			if (magnitude < 0) magnitude = defaultMagnitude;

			if (_shakeCoroutine != null)
				StopCoroutine(_shakeCoroutine);

			_shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
		}

		public void ShakeLight() => Shake(0.1f, 0.05f);
		public void ShakeMedium() => Shake(0.15f, 0.1f);
		public void ShakeHeavy() => Shake(0.25f, 0.15f);

		private IEnumerator ShakeRoutine(float duration, float magnitude)
		{
			float elapsed = 0f;

			while (elapsed < duration)
			{
				float x = Random.Range(-1f, 1f) * magnitude;
				float y = Random.Range(-1f, 1f) * magnitude;

				transform.localPosition = _originalPosition + new Vector3(x, y, 0f);

				elapsed += Time.unscaledDeltaTime;
				yield return null;
			}

			transform.localPosition = _originalPosition;
			_shakeCoroutine = null;
		}

		private void OnDisable()
		{
			if (_shakeCoroutine != null)
			{
				StopCoroutine(_shakeCoroutine);
				transform.localPosition = _originalPosition;
			}
		}
	}
}
