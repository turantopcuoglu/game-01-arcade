using UnityEngine;
using Project.Systems.Input;

public class PlayerController : MonoBehaviour
{
	[Header("Forward Movement")]
	[SerializeField] private float normalSpeed = 10f;
	[SerializeField] private float grindSpeed = 1f;
	[SerializeField] private float speedLerpRate = 8f;

	[Header("Swerve")]
	[SerializeField] private float swerveSpeed = 0.5f;
	[SerializeField] private float maxSwerveX = 3f;

	private float _currentSpeed;
	private float _targetSpeed;
	private float _swerveInput;
	private bool _isMoving;

	public float CurrentSpeed => _currentSpeed;

	private void OnEnable()
	{
		GameManagerTT.Instance.OnStateChanged += OnStateChanged;
		InputRouter.Instance.OnDragDelta += OnDrag;
	}

	private void OnDisable()
	{
		if (GameManagerTT.Instance != null)
			GameManagerTT.Instance.OnStateChanged -= OnStateChanged;
		if (InputRouter.Instance != null)
			InputRouter.Instance.OnDragDelta -= OnDrag;
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
				break;
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (!_isMoving) return;
		_swerveInput = delta.x * swerveSpeed;
	}

	private void Update()
	{
		if (!_isMoving) return;
		Debug.Log($"Moving");
		_currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, Time.deltaTime * speedLerpRate);

		// Forward movement (Z axis)
		Vector3 pos = transform.position;
		pos.z += _currentSpeed * Time.deltaTime;

		// Swerve (X axis)
		pos.x += _swerveInput * Time.deltaTime;
		pos.x = Mathf.Clamp(pos.x, -maxSwerveX, maxSwerveX);

		transform.position = pos;

		_swerveInput = 0f;
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
