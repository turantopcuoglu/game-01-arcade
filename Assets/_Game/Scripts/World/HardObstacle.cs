using UnityEngine;

/// <summary>
/// Indestructible obstacle (sawblade, spinning hazard).
/// Cannot be ground down — player must swerve around it.
/// Contact destroys scraps from the vortex without damaging the obstacle.
///
/// Setup in Unity:
/// - Add a Cylinder mesh (rotated 90° on X), red material
/// - Add a trigger Collider (slightly larger than mesh)
/// - Rigidbody isKinematic = true
/// - This component handles rotation and scrap damage
/// </summary>
public class HardObstacle : MonoBehaviour
{
	[Header("Rotation")]
	[SerializeField] private float rotationSpeed = 360f;
	[SerializeField] private Vector3 rotationAxis = Vector3.up;

	[Header("Damage")]
	[Tooltip("How many scraps are destroyed per contact")]
	[SerializeField] private int scrapDamagePerHit = 3;

	private void Update()
	{
		transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
	}

	private void OnTriggerEnter(Collider other)
	{
		var vortex = other.GetComponent<StackManager>();
		if (vortex == null) vortex = other.GetComponentInChildren<StackManager>();
		if (vortex == null) return;

		// Destroy scraps on contact
		for (int i = 0; i < scrapDamagePerHit; i++)
		{
			if (vortex.Count <= 0) break;

			ScrapItem scrap = vortex.RemoveScrap();
			if (scrap != null)
				Destroy(scrap.gameObject);
		}

		GameEvents.HardObstacleHit(transform.position);

		// Check fail condition
		if (vortex.Count <= 0 && GameManagerTT.Instance.ScrapCount <= 0)
			GameManagerTT.Instance.UpdateState(GameState.Fail);
	}
}
