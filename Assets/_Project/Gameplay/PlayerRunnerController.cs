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
		[SerializeField, Range(0.3f, 2.0f)] private float baseSensitivity = 0.8f;
		[SerializeField] private float maxDeltaPerFrame = 0.25f;
		[SerializeField] private float smoothTime = 0.08f;

		[Header("Soft Edge Clamp")]
		[SerializeField] private float softEdgeZone = 0.3f;
		[SerializeField] private float edgeResistance = 0.4f;

		[Header("Difficulty Scaling")]
		[SerializeField] private DifficultyController difficulty;
		[SerializeField] private float minSpeedMultiplier = 1.0f;
		[SerializeField] private float maxSpeedMultiplier = 1.4f;

		private float _targetX;
		private float _velX;

		private void Awake()
		{
			_targetX = transform.position.x;

			if (difficulty == null)
				difficulty = FindObjectOfType<DifficultyController>();
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
			normalized = Mathf.Clamp(normalized, -maxDeltaPerFrame, maxDeltaPerFrame);

			// Difficulty-based sensitivity scaling
			float speedMultiplier = GetSpeedMultiplier();
			float effectiveSensitivity = baseSensitivity * speedMultiplier;

			float movement = normalized * effectiveSensitivity * (maxX - minX);

			// Apply soft edge resistance
			movement = ApplySoftEdgeClamp(movement);

			_targetX += movement;
			_targetX = Mathf.Clamp(_targetX, minX, maxX);
		}

		private float ApplySoftEdgeClamp(float movement)
		{
			float currentX = _targetX;
			float edgeStart = maxX - softEdgeZone;
			float edgeStartMin = minX + softEdgeZone;

			// Check if near right edge and moving right
			if (currentX > edgeStart && movement > 0)
			{
				float edgeFactor = (currentX - edgeStart) / softEdgeZone;
				float resistance = Mathf.Lerp(1f, edgeResistance, edgeFactor);
				movement *= resistance;
			}
			// Check if near left edge and moving left
			else if (currentX < edgeStartMin && movement < 0)
			{
				float edgeFactor = (edgeStartMin - currentX) / softEdgeZone;
				float resistance = Mathf.Lerp(1f, edgeResistance, edgeFactor);
				movement *= resistance;
			}

			return movement;
		}

		private float GetSpeedMultiplier()
		{
			if (difficulty == null) return 1f;

			float normalizedSpeed = Mathf.InverseLerp(6f, 11f, difficulty.CurrentSpeed);
			return Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, normalizedSpeed);
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