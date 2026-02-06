using UnityEngine;

public class ScrapItem : MonoBehaviour, ICollectable
{
	[SerializeField] private float magnetPullSpeed = 15f;
	[SerializeField] private float orbitFollowSpeed = 10f;

	private enum State { Idle, Pulling, Orbiting }

	private State _state = State.Idle;
	private Transform _orbitCenter;
	private Vector3 _orbitTarget;
	private VortexManager _targetVortex;

	public void Collect(Transform collector)
	{
		_targetVortex = collector.GetComponent<VortexManager>();
		if (_targetVortex == null)
			_targetVortex = collector.GetComponentInChildren<VortexManager>();

		if (_targetVortex == null) return;

		_state = State.Pulling;

		var col = GetComponent<Collider>();
		if (col != null) col.enabled = false;
	}

	public void EnterOrbit(Transform center)
	{
		_orbitCenter = center;
		_state = State.Orbiting;
		transform.SetParent(null);
	}

	public void SetOrbitTarget(Vector3 worldPos)
	{
		_orbitTarget = worldPos;
	}

	private void Update()
	{
		switch (_state)
		{
			case State.Pulling:
				UpdatePull();
				break;
			case State.Orbiting:
				UpdateOrbit();
				break;
		}
	}

	private void UpdatePull()
	{
		if (_targetVortex == null) return;

		Vector3 target = _targetVortex.transform.position;
		transform.position = Vector3.MoveTowards(
			transform.position, target, magnetPullSpeed * Time.deltaTime);

		if (Vector3.Distance(transform.position, target) < 0.3f)
		{
			_targetVortex.AddScrap(this);
		}
	}

	private void UpdateOrbit()
	{
		transform.position = Vector3.Lerp(
			transform.position, _orbitTarget, orbitFollowSpeed * Time.deltaTime);
	}

	public void LaunchToward(Vector3 target, float speed)
	{
		_state = State.Idle;
		transform.SetParent(null);

		var rb = GetComponent<Rigidbody>();
		if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

		rb.isKinematic = false;
		rb.useGravity = false;
		Vector3 dir = (target - transform.position).normalized;
		rb.velocity = dir * speed;

		Destroy(gameObject, 3f);
	}
}
