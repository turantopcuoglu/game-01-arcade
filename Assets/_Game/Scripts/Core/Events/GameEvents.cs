using System;
using UnityEngine;

/// <summary>
/// Static event bus — Observer Pattern.
/// All cross-system communication goes through here.
/// No system needs a direct reference to another system.
/// </summary>
public static class GameEvents
{
	// ── Scrap ──────────────────────────────────────────
	public static event Action<ScrapItem> OnScrapCollected;
	public static event Action<int> OnScrapCountChanged;

	// ── Grinding ───────────────────────────────────────
	public static event Action OnGrindStarted;
	public static event Action OnGrindTick;
	public static event Action OnGrindStopped;

	// ── Damage / Obstacle ──────────────────────────────
	public static event Action<IDamageable, int> OnDamageTaken;
	public static event Action<IDamageable> OnObstacleDestroyed;

	// ── Gate ───────────────────────────────────────────
	public static event Action<GateOperation, int> OnGateActivated;

	// ── Hard Obstacle ──────────────────────────────────
	public static event Action<Vector3> OnHardObstacleHit;

	// ── Titan ──────────────────────────────────────────
	public static event Action<float> OnTitanProgress;
	public static event Action OnTitanComplete;

	// ── Level ──────────────────────────────────────────
	public static event Action<LevelData> OnLevelLoaded;

	// ── Fire Methods ───────────────────────────────────

	public static void ScrapCollected(ScrapItem scrap)
		=> OnScrapCollected?.Invoke(scrap);

	public static void ScrapCountUpdated(int count)
		=> OnScrapCountChanged?.Invoke(count);

	public static void GrindStarted() => OnGrindStarted?.Invoke();
	public static void GrindTick()    => OnGrindTick?.Invoke();
	public static void GrindStopped() => OnGrindStopped?.Invoke();

	public static void DamageTaken(IDamageable target, int remainingHP)
		=> OnDamageTaken?.Invoke(target, remainingHP);

	public static void ObstacleDestroyed(IDamageable target)
		=> OnObstacleDestroyed?.Invoke(target);

	public static void GateActivated(GateOperation op, int value)
		=> OnGateActivated?.Invoke(op, value);

	public static void HardObstacleHit(Vector3 position)
		=> OnHardObstacleHit?.Invoke(position);

	public static void TitanProgressChanged(float progress)
		=> OnTitanProgress?.Invoke(progress);

	public static void TitanCompleted()
		=> OnTitanComplete?.Invoke();

	public static void LevelLoaded(LevelData data)
		=> OnLevelLoaded?.Invoke(data);

	/// <summary>
	/// Call on scene unload to prevent memory leaks from static events.
	/// </summary>
	public static void ClearAll()
	{
		OnScrapCollected = null;
		OnScrapCountChanged = null;
		OnGrindStarted = null;
		OnGrindTick = null;
		OnGrindStopped = null;
		OnDamageTaken = null;
		OnObstacleDestroyed = null;
		OnGateActivated = null;
		OnHardObstacleHit = null;
		OnTitanProgress = null;
		OnTitanComplete = null;
		OnLevelLoaded = null;
	}
}
