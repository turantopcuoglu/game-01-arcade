using UnityEngine;

/// <summary>
/// Camera follow + juice effects (shake, FOV punch).
/// Follows the player from a fixed offset with smooth damping.
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

	private float _shakeMagnitude;
	private float _fovPunch;
	private float _fovPunchVelocity;

	private void Awake()
	{
		if (cam == null)
			cam = GetComponent<Camera>();
	}

	private void LateUpdate()
	{
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
