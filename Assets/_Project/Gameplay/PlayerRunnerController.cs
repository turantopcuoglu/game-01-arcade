using UnityEngine;
using Project.Systems.Input;

namespace Project.Gameplay
{
	public class PlayerRunnerController : MonoBehaviour
	{
		[Header("Bounds")]
		[SerializeField] private float minX = -2.2f;
		[SerializeField] private float maxX = 2.2f;

		[Header("Tuning")]
		[SerializeField, Range(0.3f, 2.0f)] private float sensitivity = 0.8f;
		[SerializeField] private float maxDeltaPerFrame = 0.25f; // normalized clamp
		[SerializeField] private float smoothTime = 0.10f;


		private float _targetX;
		private float _velX;

		private void Awake()
		{
			_targetX = transform.position.x;
		}

		private void OnEnable()
		{
			InputRouter.Instance.OnDragDelta += HandleDrag;
		}

		private void OnDisable()
		{
			if (InputRouter.Instance != null)
				InputRouter.Instance.OnDragDelta -= HandleDrag;
		}

		private void HandleDrag(Vector2 delta)
		{
			float normalized = delta.x / Screen.width;

			// Mouse gibi agresif inputlarý sýnýrlamak için frame clamp
			normalized = Mathf.Clamp(normalized, -maxDeltaPerFrame, maxDeltaPerFrame);

			_targetX += normalized * sensitivity * (maxX - minX);
			_targetX = Mathf.Clamp(_targetX, minX, maxX);
		}


		private void LateUpdate()
		{
			var pos = transform.position;
			pos.x = Mathf.SmoothDamp(pos.x, _targetX, ref _velX, smoothTime);
			transform.position = pos;
		}

		public void ResetController()
		{
			_velX = 0f;
			_targetX = transform.position.x;
		}
	}
}