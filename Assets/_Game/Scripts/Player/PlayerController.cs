using UnityEngine;
using Project.Systems.Input;

/// <summary>
/// Handles forward movement (auto-run) and lateral movement via dynamic touch joystick.
///
/// Touch joystick system:
///   1. Finger down anchors the joystick origin
///   2. Horizontal displacement from anchor maps to a normalized input (-1 to 1)
///   3. Dead zone prevents micro-jitter, max radius caps full-speed input
///   4. Update() applies continuous lateral velocity based on input
///   5. On finger up, lateral movement smoothly stops
/// </summary>
public class PlayerController : MonoBehaviour
{
	[Header("Forward Movement")]
	[SerializeField] private float normalSpeed = 10f;
	[SerializeField] private float grindSpeed = 1f;
	[SerializeField] private float speedLerpRate = 8f;

	[Header("Touch Joystick")]
	[Tooltip("Screen fraction for dead zone radius (no input below this).")]
	[SerializeField] private float deadZoneFraction = 0.02f;
	[Tooltip("Screen fraction for max input radius (full speed beyond this).")]
	[SerializeField] private float maxRadiusFraction = 0.12f;
	[Tooltip("Maximum lateral movement speed in world units/sec.")]
	[SerializeField] private float lateralSpeed = 8f;
	[Tooltip("How quickly lateral movement responds to input changes.")]
	[SerializeField] private float inputSmoothSpeed = 10f;
	[SerializeField] private float maxSwerveX = 3.5f;

	private float _currentSpeed;
	private float _targetSpeed;
	private bool _isMoving;

	// Joystick tracking
	private bool _isDragging;
	private float _anchorX;
	private float _joystickInput;   // Raw normalized input -1 to 1
	private float _smoothedInput;   // Smoothed input for movement

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
			case GameState.TitanCinematic:
			case GameState.Win:
			case GameState.Fail:
			case GameState.Pause:
			case GameState.Menu:
				_isMoving = false;
				_isDragging = false;
				_joystickInput = 0f;
				break;
		}
	}

	private void OnFingerDown(Vector2 screenPos)
	{
		if (!_isMoving) return;
		_isDragging = true;
		_anchorX = screenPos.x;
		_joystickInput = 0f;
	}

	private void OnPointerMove(Vector2 screenPos)
	{
		if (!_isDragging || !_isMoving) return;

		float displacement = screenPos.x - _anchorX;
		float screenWidth = Screen.width;
		float deadZone = screenWidth * deadZoneFraction;
		float maxRadius = screenWidth * maxRadiusFraction;

		float sign = Mathf.Sign(displacement);
		float abs = Mathf.Abs(displacement);

		if (abs < deadZone)
		{
			_joystickInput = 0f;
		}
		else
		{
			// Remap: deadZone..maxRadius → 0..1
			float remapped = Mathf.InverseLerp(deadZone, maxRadius, abs);
			_joystickInput = sign * remapped;
		}
	}

	private void OnFingerUp()
	{
		_isDragging = false;
		_joystickInput = 0f;
	}

	private void Update()
	{
		if (!_isMoving) return;

		// Forward speed (smooth transition between normal and grind)
		_currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, Time.deltaTime * speedLerpRate);

		// Smooth the joystick input
		_smoothedInput = Mathf.Lerp(_smoothedInput, _joystickInput, Time.deltaTime * inputSmoothSpeed);

		Vector3 pos = transform.position;

		// Forward movement (Z axis)
		pos.z += _currentSpeed * Time.deltaTime;

		// Lateral movement (X axis) — continuous velocity from joystick input
		pos.x += _smoothedInput * lateralSpeed * Time.deltaTime;
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
