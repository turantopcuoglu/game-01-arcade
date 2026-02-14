using UnityEngine;
using Project.Systems.Input;

/// <summary>
/// Handles forward movement (auto-run) and lateral swerve (touch drag).
///
/// Swerve uses a TARGET-BASED system:
///   1. Finger drag sets _targetSwerveX (accumulated delta, clamped)
///   2. Update() smoothly interpolates pos.x toward _targetSwerveX via Lerp
///   3. On finger up, character stays at current lane (smooth deceleration)
///
/// This is frame-rate independent and feels responsive on all devices.
/// </summary>
public class PlayerController : MonoBehaviour
{
	[Header("Forward Movement")]
	[SerializeField] private float normalSpeed = 10f;
	[SerializeField] private float grindSpeed = 1f;
	[SerializeField] private float speedLerpRate = 8f;

	[Header("Swerve")]
	[Tooltip("World units per full-screen drag. Match to road width for intuitive feel.")]
	[SerializeField] private float swerveSensitivity = 7f;
	[SerializeField] private float maxSwerveX = 3.5f;
	[SerializeField] private float swerveLerpSpeed = 12f;

	private float _currentSpeed;
	private float _targetSpeed;
	private bool _isMoving;

	// Swerve tracking
	private bool _isDragging;
	private float _lastPointerX;
	private float _targetSwerveX;

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
		_targetSwerveX = transform.position.x;
	}

	private void OnPointerMove(Vector2 screenPos)
	{
		if (!_isDragging || !_isMoving) return;

		float deltaX = screenPos.x - _lastPointerX;
		_lastPointerX = screenPos.x;

		_targetSwerveX += (deltaX / Screen.width) * swerveSensitivity;
		_targetSwerveX = Mathf.Clamp(_targetSwerveX, -maxSwerveX, maxSwerveX);
	}

	private void OnFingerUp()
	{
		_isDragging = false;
	}

	private void Update()
	{
		if (!_isMoving) return;

		// Forward speed (smooth transition between normal and grind)
		_currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, Time.deltaTime * speedLerpRate);

		Vector3 pos = transform.position;

		// Forward movement (Z axis)
		pos.z += _currentSpeed * Time.deltaTime;

		// Swerve (X axis) — smooth interpolation toward target
		pos.x = Mathf.Lerp(pos.x, _targetSwerveX, Time.deltaTime * swerveLerpSpeed);

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
