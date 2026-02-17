using UnityEngine;

/// <summary>
/// Camera follow + juice effects (shake, FOV punch).
/// Two modes:
///   1. Follow Mode — follows the player from a fixed offset with smooth damping.
///   2. Titan Win Mode — orbits around the Titan with a wide FOV for the win cinematic.
/// Other systems call Shake() and PunchFOV() through FeedbackManager.
/// </summary>
public class GameCamera : MonoBehaviour
{
	[Header("Follow")]
	[SerializeField] private Transform target;
	[SerializeField] private Vector3 offset = new(0f, 8f, -6f);
	[SerializeField] private float followSpeed = 5f;

	[Header("FOV")]
	[SerializeField] private Camera cam;
	[SerializeField] private float baseFOV = 60f;

	[Header("Shake Settings")]
	[SerializeField] private float shakeDecay = 5f;

	[Header("Titan Win Camera")]
	[SerializeField] private float titanFOV = 75f;
	[SerializeField] private float orbitSpeed = 40f;
	[SerializeField] private float orbitRadius = 10f;
	[SerializeField] private float orbitHeight = 5f;
	[SerializeField] private float titanCamTransitionSpeed = 2f;

	private float _shakeMagnitude;
	private float _fovPunch;
	private float _fovPunchVelocity;

	// Titan win mode
	private bool _titanWinMode;
	private Transform _titanTarget;
	private float _orbitAngle;

	private void Awake()
	{
		if (cam == null)
			cam = GetComponent<Camera>();
	}

	private void LateUpdate()
	{
		if (_titanWinMode && _titanTarget != null)
		{
			UpdateTitanWinCamera();
			return;
		}

		if (target == null) return;

		// Smooth follow
		Vector3 desiredPos = target.position + offset;
		transform.position = Vector3.Lerp(
			transform.position, desiredPos, followSpeed * Time.deltaTime);

		transform.LookAt(target.position);

		// Shake decay
		if (_shakeMagnitude > 0.01f)
		{
			transform.position += Random.insideUnitSphere * _shakeMagnitude;
			_shakeMagnitude = Mathf.Lerp(_shakeMagnitude, 0f, shakeDecay * Time.deltaTime);
		}

		// FOV punch decay
		float targetFOV = baseFOV + _fovPunch;
		cam.fieldOfView = Mathf.SmoothDamp(
			cam.fieldOfView, targetFOV, ref _fovPunchVelocity, 0.15f);
		_fovPunch = Mathf.Lerp(_fovPunch, 0f, 3f * Time.deltaTime);
	}

	private void UpdateTitanWinCamera()
	{
		// Orbit angle
		_orbitAngle += orbitSpeed * Time.deltaTime;

		float rad = _orbitAngle * Mathf.Deg2Rad;
		Vector3 orbitPos = _titanTarget.position + new Vector3(
			Mathf.Sin(rad) * orbitRadius,
			orbitHeight,
			Mathf.Cos(rad) * orbitRadius
		);

		// Smooth transition to orbit position
		transform.position = Vector3.Lerp(
			transform.position, orbitPos, titanCamTransitionSpeed * Time.deltaTime);

		// Look at Titan center (slightly above ground)
		transform.LookAt(_titanTarget.position + Vector3.up * 2f);

		// Smooth FOV transition to wide angle
		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, titanFOV, 2f * Time.deltaTime);
	}

	/// <summary>
	/// Switch to Titan win camera: orbits the Titan with a wide FOV.
	/// </summary>
	public void EnterTitanWinMode(Transform titan)
	{
		_titanWinMode = true;
		_titanTarget = titan;

		// Start orbit angle from current camera direction relative to Titan
		Vector3 dir = transform.position - titan.position;
		_orbitAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
	}

	/// <summary>
	/// Exit Titan win camera and return to normal follow mode.
	/// </summary>
	public void ExitTitanWinMode()
	{
		_titanWinMode = false;
	}

	/// <summary>
	/// Adds camera shake. Decays over time.
	/// Light: 0.05f, Medium: 0.15f, Heavy: 0.3f
	/// </summary>
	public void Shake(float magnitude)
	{
		_shakeMagnitude = Mathf.Max(_shakeMagnitude, magnitude);
	}

	/// <summary>
	/// Temporarily widens FOV for a "whoosh" effect.
	/// Typical value: 5-10f
	/// </summary>
	public void PunchFOV(float amount)
	{
		_fovPunch += amount;
	}

	public void SetTarget(Transform newTarget)
	{
		target = newTarget;
	}
}
