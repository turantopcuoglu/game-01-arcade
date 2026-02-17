using System.Collections;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
	[Header("Titan Settings")]
	[SerializeField] private Transform titanTarget;
	[SerializeField] private float scrapLaunchSpeed = 20f;
	[SerializeField] private float launchInterval = 0.05f;

	[Header("Win Sequence")]
	[SerializeField] private GameCamera gameCamera;
	[SerializeField] private float winPanelDelay = 1.5f;
	[SerializeField] private float titanCompleteTimeout = 5f;

	private bool _triggered;

	private void OnTriggerEnter(Collider other)
	{
		if (_triggered) return;

		var player = other.GetComponent<PlayerController>();
		if (player == null) return;

		var vortex = other.GetComponentInChildren<StackManager>();
		if (vortex == null) vortex = other.GetComponent<StackManager>();
		if (vortex == null) return;

		_triggered = true;

		// Enter cinematic state — player stops, no panel shown yet
		GameManagerTT.Instance.UpdateState(GameState.TitanCinematic);

		StartCoroutine(TitanWinSequence(vortex));
	}

	private IEnumerator TitanWinSequence(StackManager vortex)
	{
		// Switch camera to Titan orbit mode
		if (gameCamera != null && titanTarget != null)
			gameCamera.EnterTitanWinMode(titanTarget);

		// Launch scraps toward the Titan one by one
		var scraps = vortex.ReleaseAll();
		var wait = new WaitForSeconds(launchInterval);

		foreach (var scrap in scraps)
		{
			if (scrap == null) continue;
			scrap.LaunchToward(titanTarget.position, scrapLaunchSpeed);
			yield return wait;
		}

		// Wait for Titan to finish building (or timeout)
		bool titanDone = false;
		System.Action onTitanComplete = () => titanDone = true;
		GameEvents.OnTitanComplete += onTitanComplete;

		float elapsed = 0f;
		while (!titanDone && elapsed < titanCompleteTimeout)
		{
			elapsed += Time.deltaTime;
			yield return null;
		}

		GameEvents.OnTitanComplete -= onTitanComplete;

		// Post-completion celebration delay (camera keeps orbiting)
		yield return new WaitForSeconds(winPanelDelay);

		// Now trigger the actual Win state — panel appears
		GameManagerTT.Instance.UpdateState(GameState.Win);
	}
}
