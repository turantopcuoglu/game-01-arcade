using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FinishLine : MonoBehaviour, IAutoBindable
{
	[Header("Titan Settings")]
	[SerializeField] private Transform titanTarget;
	[SerializeField] private float scrapLaunchSpeed = 20f;
	[SerializeField] private float launchInterval = 0.05f;

	[Header("Cinemachine")]
	[SerializeField] private CinemachineVirtualCamera titanCam;
	[SerializeField] private int finishingCamPriority = 20;

	[Header("Win Delay")]
	[Tooltip("Extra seconds to wait after all scraps are delivered before showing the win panel.")]
	[SerializeField] private float winPanelDelay = 0.5f;

	private bool _triggered;
	private int _originalCamPriority;

	public void AutoBind(GameObject levelRoot)
	{
		// Resolve titanCam: search level first, then scene-wide
		if (titanCam == null)
			titanCam = levelRoot.GetComponentInChildren<CinemachineVirtualCamera>();
		if (titanCam == null)
			titanCam = FindObjectOfType<CinemachineVirtualCamera>();

		if (titanCam != null)
			_originalCamPriority = titanCam.Priority;

		// Resolve titanTarget: find TitanController's transform
		if (titanTarget == null)
		{
			var titan = levelRoot.GetComponentInChildren<TitanController>();
			if (titan == null) titan = FindObjectOfType<TitanController>();
			if (titan != null) titanTarget = titan.transform;
		}
	}

	/// <summary>
	/// Resets titanCam priority to its original value.
	/// Must be called synchronously before Destroy (OnDestroy is deferred
	/// and would fire after the new level's AutoBind reads the dirty priority).
	/// </summary>
	public void ResetCamera()
	{
		if (titanCam != null)
			titanCam.Priority = _originalCamPriority;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_triggered) return;

		var player = other.GetComponent<PlayerController>();
		if (player == null) return;

		var vortex = other.GetComponentInChildren<StackManager>();
		if (vortex == null) vortex = other.GetComponent<StackManager>();
		if (vortex == null) return;

		_triggered = true;

		// Enter Finishing state — player stops, HUD stays, no win panel yet
		GameManagerTT.Instance.UpdateState(GameState.Finishing);

		// Switch Cinemachine to titan camera
		if (titanCam != null)
			titanCam.Priority = finishingCamPriority;

		if (titanTarget != null)
			StartCoroutine(LaunchScrapsToTitan(vortex));
		else
			StartCoroutine(DelayedWin(0f));
	}

	private IEnumerator LaunchScrapsToTitan(StackManager vortex)
	{
		var scraps = vortex.ReleaseAll();
		var wait = new WaitForSeconds(launchInterval);
		var launched = new List<ScrapItem>();

		foreach (var scrap in scraps)
		{
			if (scrap == null) continue;
			scrap.LaunchToward(titanTarget.position, scrapLaunchSpeed);
			launched.Add(scrap);
			yield return wait;
		}

		// Wait until all launched scraps have been destroyed (arrived at titan or timed out)
		if (launched.Count > 0)
		{
			yield return new WaitUntil(() => AllScrapsDelivered(launched));
		}

		GameEvents.AllScrapsDelivered();

		yield return DelayedWin(winPanelDelay);
	}

	private IEnumerator DelayedWin(float delay)
	{
		if (delay > 0f)
			yield return new WaitForSeconds(delay);

		GameManagerTT.Instance.UpdateState(GameState.Win);
	}

	private bool AllScrapsDelivered(List<ScrapItem> scraps)
	{
		for (int i = 0; i < scraps.Count; i++)
		{
			if (scraps[i] != null) return false;
		}
		return true;
	}
}
