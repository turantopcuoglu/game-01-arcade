using UnityEngine;
using Project.Systems.Input;

public class PlayerController : MonoBehaviour
{
	[Header("Forward Movement")]
	[SerializeField] private float normalSpeed = 10f;
	[SerializeField] private float grindSpeed = 1f;
	[SerializeField] private float speedLerpRate = 8f;

	[Header("Swerve")]
	[SerializeField] private float swerveMultiplier = 6f;
	[SerializeField] private float maxSwerveX = 3f;
	[SerializeField] private float swerveSmoothRate = 12f;

	private float _currentSpeed;
	private float _targetSpeed;
	private bool _isMoving;

	// Swerve tracking
	private bool _isDragging;
	private float _lastPointerX;
	private float _swerveVelocity;

	public float CurrentSpeed => _currentSpeed;

	private void OnEnable()
	{
		GameManagerTT.Instance.OnStateChanged += OnStateChanged;
		InputRouter.Instance.OnTapScreen += OnFingerDown;
		InputRouter.Instance.OnPointerScreen += OnPointerMove;
		InputRouter.Instance.OnFingerUp += OnFingerUp;
	}

	private void OnDisable()
	{
		if (GameManagerTT.Instance != null)
			GameManagerTT.Instance.OnStateChanged -= OnStateChanged;
		if (InputRouter.Instance != null)
		{
			InputRouter.Instance.OnTapScreen -= OnFingerDown;
			InputRouter.Instance.OnPointerScreen -= OnPointerMove;
			InputRouter.Instance.OnFingerUp -= OnFingerUp;
		}
	}

	private void OnStateChanged(GameState state)
	{
		switch (state)
		{
			case GameState.Gameplay:
				_isMoving = true;
				_targetSpeed = normalSpeed;
				break;
			case GameState.Grinding:
				_isMoving = true;
				_targetSpeed = grindSpeed;
				break;
			case GameState.Win:
			case GameState.Fail:
			case GameState.Pause:
			case GameState.Menu:
				_isMoving = false;
				_isDragging = false;
				break;
		}
	}

	private void OnFingerDown(Vector2 screenPos)
	{
		if (!_isMoving) return;
		_isDragging = true;
		_lastPointerX = screenPos.x;
	}

	private void OnPointerMove(Vector2 screenPos)
	{
		if (!_isDragging || !_isMoving) return;

		float deltaX = screenPos.x - _lastPointerX;
		_lastPointerX = screenPos.x;

		// Normalize by screen width so swerve feels consistent across resolutions
		_swerveVelocity = (deltaX / Screen.width) * swerveMultiplier;
	}

	private void OnFingerUp()
	{
		_isDragging = false;
		_swerveVelocity = 0f;
	}

	private void Update()
	{
		if (!_isMoving) return;
    
		_currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, Time.deltaTime * speedLerpRate);

		Vector3 pos = transform.position;

		// Forward movement (Z axis)
		pos.z += _currentSpeed * Time.deltaTime;

		// Swerve (X axis) - direct position offset, smoothed
		pos.x += _swerveVelocity;
		pos.x = Mathf.Clamp(pos.x, -maxSwerveX, maxSwerveX);

		transform.position = pos;
	}

	public void SetGrinding(bool grinding)
	{
		if (grinding && GameManagerTT.Instance.CurrentState == GameState.Gameplay)
		{
			GameManagerTT.Instance.UpdateState(GameState.Grinding);
		}
		else if (!grinding && GameManagerTT.Instance.CurrentState == GameState.Grinding)
		{
			GameManagerTT.Instance.UpdateState(GameState.Gameplay);
		}
	}
}
