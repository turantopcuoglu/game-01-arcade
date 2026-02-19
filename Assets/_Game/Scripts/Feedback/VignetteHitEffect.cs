using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Flashes the Global Volume vignette (red) on collision/damage events,
/// then smoothly fades it back to zero.
/// Attach to any persistent GameObject and assign the scene's Global Volume.
/// </summary>
public class VignetteHitEffect : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Volume globalVolume;

	[Header("Intensities")]
	[SerializeField] private float hardHitIntensity = 0.55f;
	[SerializeField] private float grindTickIntensity = 0.2f;
	[SerializeField] private float negativeGateIntensity = 0.35f;

	[Header("Fade")]
	[SerializeField] private float fadeSpeed = 5f;

	private Vignette _vignette;
	private float _currentIntensity;

	private void Awake()
	{
		if (globalVolume != null && globalVolume.profile.TryGet(out Vignette v))
			_vignette = v;
	}

	private void OnEnable()
	{
		GameEvents.OnHardObstacleHit += HandleHardHit;
		GameEvents.OnGrindTick += HandleGrindTick;
		GameEvents.OnGateActivated += HandleGateActivated;
	}

	private void OnDisable()
	{
		GameEvents.OnHardObstacleHit -= HandleHardHit;
		GameEvents.OnGrindTick -= HandleGrindTick;
		GameEvents.OnGateActivated -= HandleGateActivated;
		ResetVignette();
	}

	private void Update()
	{
		if (_vignette == null) return;

		_currentIntensity = Mathf.Lerp(_currentIntensity, 0f, fadeSpeed * Time.deltaTime);

		if (_currentIntensity < 0.01f)
			_currentIntensity = 0f;

		_vignette.intensity.value = _currentIntensity;
	}

	// ── Event Handlers ─────────────────────────────────

	private void HandleHardHit(Vector3 position)
	{
		Flash(hardHitIntensity);
	}

	private void HandleGrindTick()
	{
		Flash(grindTickIntensity);
	}

	private void HandleGateActivated(GateOperation op, int value)
	{
		bool isNegative = op == GateOperation.Subtract || op == GateOperation.Divide;
		if (isNegative)
			Flash(negativeGateIntensity);
	}

	// ── Helpers ────────────────────────────────────────

	private void Flash(float intensity)
	{
		_currentIntensity = Mathf.Max(_currentIntensity, intensity);
	}

	private void ResetVignette()
	{
		if (_vignette != null)
		{
			_currentIntensity = 0f;
			_vignette.intensity.value = 0f;
		}
	}
}
