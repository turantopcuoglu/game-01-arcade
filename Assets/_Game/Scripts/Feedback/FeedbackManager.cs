using UnityEngine;
using Project.Audio;
using Project.Systems.Haptics;

/// <summary>
/// Mediator Pattern — centralized juice/feedback coordinator.
/// Listens to GameEvents and triggers the appropriate feedback:
/// camera shake, haptics, audio, particles.
///
/// No other gameplay script needs to know about AudioService or Haptics.
/// All feedback flows through here.
/// </summary>
public class FeedbackManager : MonoBehaviour
{
	public static FeedbackManager Instance { get; private set; }

	[Header("References")]
	[SerializeField] private GameCamera gameCamera;

	[Header("Shake Intensities")]
	[SerializeField] private float grindTickShake = 0.04f;
	[SerializeField] private float obstacleBreakShake = 0.2f;
	[SerializeField] private float hardObstacleHitShake = 0.15f;

	[Header("FOV Punch")]
	[SerializeField] private float obstacleBreakFOV = 8f;

	[Header("Slow Motion")]
	[SerializeField] private float breakSlowDuration = 0.08f;
	[SerializeField] private float breakSlowScale = 0.3f;

	private float _slowMotionTimer;

	private void Awake()
	{
		if (Instance != null) { Destroy(gameObject); return; }
		Instance = this;
	}

	private void OnEnable()
	{
		GameEvents.OnScrapCollected += HandleScrapCollected;
		GameEvents.OnGrindTick += HandleGrindTick;
		GameEvents.OnGrindStarted += HandleGrindStarted;
		GameEvents.OnGrindStopped += HandleGrindStopped;
		GameEvents.OnObstacleDestroyed += HandleObstacleDestroyed;
		GameEvents.OnGateActivated += HandleGateActivated;
		GameEvents.OnHardObstacleHit += HandleHardObstacleHit;
		GameEvents.OnTitanProgress += HandleTitanProgress;
	}

	private void OnDisable()
	{
		GameEvents.OnScrapCollected -= HandleScrapCollected;
		GameEvents.OnGrindTick -= HandleGrindTick;
		GameEvents.OnGrindStarted -= HandleGrindStarted;
		GameEvents.OnGrindStopped -= HandleGrindStopped;
		GameEvents.OnObstacleDestroyed -= HandleObstacleDestroyed;
		GameEvents.OnGateActivated -= HandleGateActivated;
		GameEvents.OnHardObstacleHit -= HandleHardObstacleHit;
		GameEvents.OnTitanProgress -= HandleTitanProgress;
	}

	private void Update()
	{
		if (_slowMotionTimer > 0f)
		{
			_slowMotionTimer -= Time.unscaledDeltaTime;
			if (_slowMotionTimer <= 0f)
				Time.timeScale = 1f;
		}
	}

	// ── Event Handlers ─────────────────────────────────

	private void HandleScrapCollected(ScrapItem scrap)
	{
		Haptics.CoinPickup();
		AudioService.Instance?.PlayCoinPickup();
	}

	private void HandleGrindStarted()
	{
		// Could start a grinding loop sound here
	}

	private void HandleGrindTick()
	{
		Haptics.Impact();
		gameCamera?.Shake(grindTickShake);
	}

	private void HandleGrindStopped()
	{
		// Could stop the grinding loop sound here
	}

	private void HandleObstacleDestroyed(IDamageable target)
	{
		// Heavy feedback burst
		Haptics.Impact();
		AudioService.Instance?.PlayCollision();
		gameCamera?.Shake(obstacleBreakShake);
		gameCamera?.PunchFOV(obstacleBreakFOV);

		// Brief slow-motion snap
		Time.timeScale = breakSlowScale;
		_slowMotionTimer = breakSlowDuration;
	}

	private void HandleGateActivated(GateOperation op, int value)
	{
		bool isPositive = op == GateOperation.Add || op == GateOperation.Multiply;

		if (isPositive)
		{
			Haptics.CoinPickup();
			AudioService.Instance?.PlayCoinPickup();
		}
		else
		{
			Haptics.Impact();
		}
	}

	private void HandleHardObstacleHit(Vector3 position)
	{
		Haptics.Impact();
		gameCamera?.Shake(hardObstacleHitShake);
	}

	private void HandleTitanProgress(float progress)
	{
		Haptics.Click();
		AudioService.Instance?.PlayClick();
	}
}
