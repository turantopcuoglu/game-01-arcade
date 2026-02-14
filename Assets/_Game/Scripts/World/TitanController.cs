using UnityEngine;

/// <summary>
/// Level-end boss that gets built from incoming scraps.
/// Placed at the end of the level, behind the FinishLine.
/// Has a trigger collider that catches launched scraps.
///
/// Body parts enable progressively as scraps arrive.
/// Fires events for UI (progress bar) and feedback (sound/haptic per scrap).
/// </summary>
public class TitanController : MonoBehaviour
{
	[Header("Build Settings")]
	[SerializeField] private int totalCapacity = 50;

	[Header("Body Parts (enable order: feet → head)")]
	[SerializeField] private Transform[] bodyParts;

	private int _receivedCount;
	private bool _completed;

	public float Progress => totalCapacity > 0
		? (float)_receivedCount / totalCapacity
		: 0f;

	public void SetCapacity(int capacity)
	{
		totalCapacity = capacity;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_completed) return;

		var scrap = other.GetComponent<ScrapItem>();
		if (scrap == null) return;

		ReceiveScrap();
		Destroy(scrap.gameObject);
	}

	private void ReceiveScrap()
	{
		Debug.Log("Titan received scrap! Progress: " + Progress);
		_receivedCount++;

		// Enable body parts progressively
		if (bodyParts != null && bodyParts.Length > 0)
		{
			int partsToEnable = Mathf.CeilToInt(Progress * bodyParts.Length);
			for (int i = 0; i < bodyParts.Length; i++)
			{
				if (bodyParts[i] != null)
					bodyParts[i].gameObject.SetActive(i < partsToEnable);
			}
		}

		GameEvents.TitanProgressChanged(Progress);

		if (_receivedCount >= totalCapacity && !_completed)
		{
			_completed = true;
			GameEvents.TitanCompleted();
		}
	}
}
