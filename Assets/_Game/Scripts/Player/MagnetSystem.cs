using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class MagnetSystem : MonoBehaviour
{
	[SerializeField] private float magnetRadius = 5f;

	private SphereCollider _trigger;

	private void Awake()
	{
		_trigger = GetComponent<SphereCollider>();
		_trigger.isTrigger = true;
		_trigger.radius = magnetRadius;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (GameManagerTT.Instance.CurrentState != GameState.Gameplay &&
		    GameManagerTT.Instance.CurrentState != GameState.Grinding)
			return;

		var collectable = other.GetComponent<ICollectable>();
		if (collectable != null)
		{
			collectable.Collect(transform.parent ?? transform);
		}
	}

	public void SetRadius(float radius)
	{
		magnetRadius = radius;
		if (_trigger != null)
			_trigger.radius = magnetRadius;
	}
}
