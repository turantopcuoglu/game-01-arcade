using UnityEngine;
using Project.Gameplay.Pooling;

namespace Project.Gameplay
{
	public enum ObstacleMovementType
	{
		Static,    // Just moves forward (backward in world space)
		Sweeper    // Moves left-right while moving forward
	}

	public class ObstacleMover : MonoBehaviour
	{
		private SimplePool _pool;
		private float _speed;
		private float _despawnZ;

		// Movement type
		private ObstacleMovementType _movementType = ObstacleMovementType.Static;

		// Sweeper settings
		private float _sweeperSpeed;
		private float _sweeperRange;
		private float _minX;
		private float _maxX;
		private float _startX;
		private float _sweeperDirection = 1f;
		private float _sweeperOffset;

		public void Init(SimplePool pool, float speed, float despawnZ)
		{
			_pool = pool;
			_speed = speed;
			_despawnZ = despawnZ;

			// Reset to default
			_movementType = ObstacleMovementType.Static;
			_sweeperOffset = 0f;
			_sweeperDirection = Random.value > 0.5f ? 1f : -1f;
		}

		/// <summary>
		/// Set the movement type and parameters.
		/// </summary>
		public void SetMovementType(ObstacleMovementType type, float sweeperSpeed = 2f, float sweeperRange = 2f, float minX = -2.2f, float maxX = 2.2f)
		{
			_movementType = type;
			_sweeperSpeed = sweeperSpeed;
			_sweeperRange = sweeperRange;
			_minX = minX;
			_maxX = maxX;
			_startX = transform.position.x;
			_sweeperOffset = 0f;
		}

		private void Update()
		{
			float dt = Time.deltaTime;

			// Forward movement (toward player)
			Vector3 pos = transform.position;
			pos.z -= _speed * dt;

			// Lateral movement for sweeper
			if (_movementType == ObstacleMovementType.Sweeper)
			{
				_sweeperOffset += _sweeperSpeed * _sweeperDirection * dt;

				// Bounce at range limits
				if (Mathf.Abs(_sweeperOffset) >= _sweeperRange / 2f)
				{
					_sweeperDirection = -_sweeperDirection;
					_sweeperOffset = Mathf.Clamp(_sweeperOffset, -_sweeperRange / 2f, _sweeperRange / 2f);
				}

				// Also clamp to lane bounds
				float targetX = _startX + _sweeperOffset;
				if (targetX < _minX || targetX > _maxX)
				{
					_sweeperDirection = -_sweeperDirection;
					targetX = Mathf.Clamp(targetX, _minX, _maxX);
				}

				pos.x = targetX;
			}

			transform.position = pos;

			// Despawn check
			if (pos.z <= _despawnZ)
			{
				// Reset scale before returning to pool
				transform.localScale = Vector3.one;

				if (_pool != null) _pool.Return(gameObject);
				else gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Update speed dynamically (for rush segments).
		/// </summary>
		public void UpdateSpeed(float newSpeed)
		{
			_speed = newSpeed;
		}
	}
}
