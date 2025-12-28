using System.Collections;
using UnityEngine;
using Project.Systems.Input;

namespace Project.Gameplay
{
	public class PlayerDummyDragController : MonoBehaviour
	{
		[Header("Tuning")]
		[SerializeField] private float sensitivity = 0.02f; // delta -> world
		[SerializeField] private float smoothTime = 0.08f;
		[SerializeField] private float minX = -2.2f;
		[SerializeField] private float maxX = 2.2f;

		private float _targetX;
		private float _vel;
		private Coroutine _subRoutine;

		private void Awake()
		{
			_targetX = transform.position.x;
		}

		private void OnEnable() => InputRouter.Instance.OnDragDelta += HandleDrag;

		private void OnDisable()
		{
			if (InputRouter.Instance != null)
				InputRouter.Instance.OnDragDelta -= HandleDrag;
		}

		private IEnumerator SubscribeWhenReady()
		{
			while (InputRouter.Instance == null)
				yield return null;

			InputRouter.Instance.OnDragDelta += HandleDrag;
			//Debug.Log("[Dummy] Subscribed to InputRouter");
		}

		private void HandleDrag(Vector2 delta)
		{
			// delta.x = pixel cinsinden. Bunu ekran geniþliðine bölelim (0..1)
			float normalized = delta.x / Screen.width;

			// maxSpeedPerSecond: saniyede max kaç world unit kayabilsin
			// dt ile çarpýp frame baðýmsýz yapýyoruz
			const float maxSpeedPerSecond = 5f;

			_targetX += normalized * maxSpeedPerSecond;
			_targetX = Mathf.Clamp(_targetX, minX, maxX);
		}


		private void LateUpdate()
		{
			var pos = transform.position;
			pos.x = Mathf.SmoothDamp(pos.x, _targetX, ref _vel, smoothTime);
			transform.position = pos;
		}

	}
}
