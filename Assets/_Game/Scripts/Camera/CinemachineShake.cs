using UnityEngine;
using Cinemachine;

/// <summary>
/// Cinemachine Impulse-based camera shake + FOV punch.
/// Replaces the old procedural GameCamera shake.
/// Requires a CinemachineImpulseSource on the same GameObject.
/// Virtual cameras need a CinemachineImpulseListener extension to receive shakes.
/// </summary>
[RequireComponent(typeof(CinemachineImpulseSource))]
public class CinemachineShake : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private CinemachineImpulseSource impulseSource;
	[SerializeField] private CinemachineVirtualCamera virtualCamera;

	[Header("FOV Punch")]
	[SerializeField] private float baseFOV = 60f;

	private float _fovPunch;
	private float _fovPunchVelocity;

	private void Awake()
	{
		if (impulseSource == null)
			impulseSource = GetComponent<CinemachineImpulseSource>();
	}

	private void LateUpdate()
	{
		if (virtualCamera == null || _fovPunch == 0f && _fovPunchVelocity == 0f) return;

		float currentFOV = virtualCamera.m_Lens.FieldOfView;
		float targetFOV = baseFOV + _fovPunch;
		virtualCamera.m_Lens.FieldOfView = Mathf.SmoothDamp(
			currentFOV, targetFOV, ref _fovPunchVelocity, 0.15f);
		_fovPunch = Mathf.Lerp(_fovPunch, 0f, 3f * Time.deltaTime);
	}

	/// <summary>
	/// Generates a Cinemachine impulse with the given force.
	/// Light: 0.05f, Medium: 0.15f, Heavy: 0.3f
	/// </summary>
	public void Shake(float force)
	{
		impulseSource.GenerateImpulseWithForce(force);
	}

	/// <summary>
	/// Temporarily widens FOV for a "whoosh" effect.
	/// Typical value: 5-10f
	/// </summary>
	public void PunchFOV(float amount)
	{
		_fovPunch += amount;
	}
}
