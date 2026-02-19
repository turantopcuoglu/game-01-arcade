using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Diamond-wipe scene transition using a 45°-rotated black image.
///
/// Animation flow (curtain style):
///   1. Cover  — image slides UP from below the screen to center (covers screen)
///   2. Callback fires while the screen is fully covered
///   3. Reveal — image slides back DOWN to its hidden position (reveals screen)
///
/// Two public APIs:
///   Play(callback)          — cover → callback → reveal  (for state changes)
///   PlayWithSceneReload()   — cover → reload scene → auto-reveal in new scene
///
/// Setup in Unity:
///   - Canvas > PassingEffectRoot > BlackPassingImage
///   - BlackPassingImage: large black Image, RectTransform rotated 45° on Z
///   - Assign BlackPassingImage's RectTransform to the passingImage field
///   - Image should be large enough to cover the full screen when centered
///     (recommended: max(screenW, screenH) * 1.5)
/// </summary>
public class PassingEffect : MonoBehaviour
{
	public static PassingEffect Instance { get; private set; }

	[Header("References")]
	[SerializeField] private RectTransform passingImage;

	[Header("Timing")]
	[SerializeField] private float coverDuration = 0.45f;
	[SerializeField] private float revealDuration = 0.45f;
	[SerializeField] private float coveredPause = 0.15f;

	[Header("Easing")]
	[SerializeField] private AnimationCurve coverCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
	[SerializeField] private AnimationCurve revealCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	private float _hiddenY;
	private float _coveredY;
	private bool _isPlaying;

	// Persists across scene loads via static field
	private static bool _revealOnStart;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	private void Start()
	{
		CalculatePositions();

		if (_revealOnStart)
		{
			_revealOnStart = false;
			SetY(_coveredY);
			passingImage.gameObject.SetActive(true);
			StartCoroutine(RevealRoutine(null));
		}
		else
		{
			SetY(_hiddenY);
			passingImage.gameObject.SetActive(false);
		}
	}

	private void CalculatePositions()
	{
		Canvas canvas = passingImage.GetComponentInParent<Canvas>();
		if (canvas == null) return;

		RectTransform canvasRect = canvas.GetComponent<RectTransform>();
		float canvasH = canvasRect.rect.height;

		// Account for 45° rotation: visual footprint ≈ size * √2
		float imgSize = Mathf.Max(passingImage.rect.width, passingImage.rect.height);
		float visualSize = imgSize * 1.5f;

		_hiddenY = -(canvasH + visualSize) / 2f;
		_coveredY = 0f;
	}

	// ── Public API ─────────────────────────────────────

	/// <summary>
	/// Cover → callback → reveal.  Use for state changes without scene reload.
	/// </summary>
	public void Play(Action onScreenCovered = null)
	{
		if (_isPlaying) return;
		StartCoroutine(FullTransitionRoutine(onScreenCovered));
	}

	/// <summary>
	/// Cover → reload active scene.  New scene auto-reveals on Start.
	/// </summary>
	public void PlayWithSceneReload()
	{
		if (_isPlaying) return;
		StartCoroutine(CoverAndReloadRoutine());
	}

	/// <summary>
	/// Returns true while a transition is in progress.
	/// </summary>
	public bool IsPlaying => _isPlaying;

	// ── Coroutines ─────────────────────────────────────

	private IEnumerator FullTransitionRoutine(Action onScreenCovered)
	{
		_isPlaying = true;
		passingImage.gameObject.SetActive(true);

		yield return CoverRoutine();

		onScreenCovered?.Invoke();

		yield return new WaitForSecondsRealtime(coveredPause);

		yield return RevealRoutine(() => _isPlaying = false);
	}

	private IEnumerator CoverAndReloadRoutine()
	{
		_isPlaying = true;
		passingImage.gameObject.SetActive(true);

		yield return CoverRoutine();

		_revealOnStart = true;

		UnityEngine.SceneManagement.SceneManager.LoadScene(
			UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
	}

	private IEnumerator CoverRoutine()
	{
		float elapsed = 0f;
		while (elapsed < coverDuration)
		{
			elapsed += Time.unscaledDeltaTime;
			float t = coverCurve.Evaluate(Mathf.Clamp01(elapsed / coverDuration));
			SetY(Mathf.Lerp(_hiddenY, _coveredY, t));
			yield return null;
		}
		SetY(_coveredY);
	}

	private IEnumerator RevealRoutine(Action onComplete)
	{
		float elapsed = 0f;
		while (elapsed < revealDuration)
		{
			elapsed += Time.unscaledDeltaTime;
			float t = revealCurve.Evaluate(Mathf.Clamp01(elapsed / revealDuration));
			SetY(Mathf.Lerp(_coveredY, _hiddenY, t));
			yield return null;
		}

		SetY(_hiddenY);
		passingImage.gameObject.SetActive(false);
		onComplete?.Invoke();
	}

	// ── Helpers ────────────────────────────────────────

	private void SetY(float y)
	{
		Vector2 pos = passingImage.anchoredPosition;
		pos.y = y;
		passingImage.anchoredPosition = pos;
	}
}
