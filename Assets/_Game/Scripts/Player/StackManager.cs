using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the scrap vortex orbiting around the player.
/// Handles orbit math (Sin/Cos layered rings), scrap add/remove,
/// gate operations with visual spawning, and release-all for Titan.
///
/// File is named StackManager.cs for historical reasons.
/// Class name is VortexManager — rename file in Unity if desired.
/// </summary>
public class VortexManager : MonoBehaviour
{
	[Header("Orbit Settings")]
	[SerializeField] private float baseOrbitRadius = 1.5f;
	[SerializeField] private float radiusPerLayer = 0.8f;
	[SerializeField] private float orbitSpeed = 180f;
	[SerializeField] private float orbitHeight = 0.5f;
	[SerializeField] private int scrapsPerLayer = 8;

	[Header("Spawning (for Gate + operations)")]
	[SerializeField] private GameObject scrapPrefab;

	private readonly List<ScrapItem> _scraps = new List<ScrapItem>();
	private float _orbitAngle;

	public int Count => _scraps.Count;

	public event Action<int> OnScrapRemoved;

	private void Update()
	{
		if (_scraps.Count == 0) return;

		_orbitAngle += orbitSpeed * Time.deltaTime;

		for (int i = _scraps.Count - 1; i >= 0; i--)
		{
			if (_scraps[i] == null)
			{
				_scraps.RemoveAt(i);
				continue;
			}

			int layer = i / scrapsPerLayer;
			int indexInLayer = i % scrapsPerLayer;
			int countInLayer = Mathf.Min(scrapsPerLayer, _scraps.Count - layer * scrapsPerLayer);

			float radius = baseOrbitRadius + layer * radiusPerLayer;
			float angleOffset = (360f / countInLayer) * indexInLayer;
			float angle = (_orbitAngle + angleOffset) * Mathf.Deg2Rad;

			Vector3 targetLocal = new Vector3(
				Mathf.Cos(angle) * radius,
				orbitHeight,
				Mathf.Sin(angle) * radius
			);

			_scraps[i].SetOrbitTarget(transform.position + targetLocal);
		}
	}

	// ── Add / Remove ───────────────────────────────────

	public void AddScrap(ScrapItem scrap)
	{
		_scraps.Add(scrap);
		scrap.EnterOrbit(transform);

		GameManagerTT.Instance.AddScrap();
		GameEvents.ScrapCollected(scrap);
	}

	public ScrapItem RemoveScrap()
	{
		if (_scraps.Count == 0) return null;

		int lastIndex = _scraps.Count - 1;
		ScrapItem scrap = _scraps[lastIndex];
		_scraps.RemoveAt(lastIndex);

		GameManagerTT.Instance.TryConsumeScrap();
		OnScrapRemoved?.Invoke(_scraps.Count);

		return scrap;
	}

	// ── Gate Operations ────────────────────────────────

	public void ApplyGateOperation(GateOperation op, int value)
	{
		int currentCount = _scraps.Count;
		int targetCount;

		switch (op)
		{
			case GateOperation.Add:
				targetCount = currentCount + value;
				break;
			case GateOperation.Subtract:
				targetCount = Mathf.Max(0, currentCount - value);
				break;
			case GateOperation.Multiply:
				targetCount = currentCount * value;
				break;
			case GateOperation.Divide:
				targetCount = value > 0 ? currentCount / value : currentCount;
				break;
			default:
				return;
		}

		int diff = targetCount - currentCount;

		if (diff > 0)
		{
			SpawnScraps(diff);
		}
		else if (diff < 0)
		{
			for (int i = 0; i < Mathf.Abs(diff) && _scraps.Count > 0; i++)
			{
				ScrapItem scrap = RemoveScrap();
				if (scrap != null)
					Destroy(scrap.gameObject);
			}
		}

		GameEvents.GateActivated(op, value);
	}

	/// <summary>
	/// Spawns new scrap instances and adds them to orbit.
	/// Used by gate + operations to create visual scraps.
	/// </summary>
	private void SpawnScraps(int count)
	{
		if (scrapPrefab == null)
		{
			// Fallback: just update the count without visual scraps
			for (int i = 0; i < count; i++)
				GameManagerTT.Instance.AddScrap();
			return;
		}

		for (int i = 0; i < count; i++)
		{
			Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
			GameObject obj = Instantiate(scrapPrefab, spawnPos, Quaternion.identity);
			ScrapItem scrap = obj.GetComponent<ScrapItem>();

			if (scrap != null)
			{
				AddScrap(scrap);
			}
		}
	}

	// ── Release All (for Titan launch) ─────────────────

	public List<ScrapItem> ReleaseAll()
	{
		var released = new List<ScrapItem>(_scraps);
		_scraps.Clear();
		return released;
	}
}
