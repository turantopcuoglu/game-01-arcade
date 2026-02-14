using System.Collections;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
	[Header("Titan Settings")]
	[SerializeField] private Transform titanTarget;
	[SerializeField] private float scrapLaunchSpeed = 20f;
	[SerializeField] private float launchInterval = 0.05f;

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
		GameManagerTT.Instance.UpdateState(GameState.Win);

		if (titanTarget != null)
			StartCoroutine(LaunchScrapsToTitan(vortex));
	}

	private IEnumerator LaunchScrapsToTitan(StackManager vortex)
	{
		var scraps = vortex.ReleaseAll();
		var wait = new WaitForSeconds(launchInterval);

		foreach (var scrap in scraps)
		{
			if (scrap == null) continue;
			scrap.LaunchToward(titanTarget.position, scrapLaunchSpeed);
			yield return wait;
		}
	}
}
