using UnityEngine;
using Project.Audio;
using Project.Systems.Haptics;

public class ObstacleBase : MonoBehaviour
{
	[Header("Stats")]
	[SerializeField] private int maxHP = 2;
	[SerializeField] private bool isIndestructible = false;

	[Header("Grinding")]
	[SerializeField] private float grindInterval = 0.1f;

	[Header("Break Effect")]
	[SerializeField] private float explosionForce = 300f;
	[SerializeField] private float explosionRadius = 3f;

	private int _currentHP;
	private bool _isGrinding;
	private float _grindTimer;
	private VortexManager _grindingVortex;
	private PlayerController _grindingPlayer;

	private void Awake()
	{
		_currentHP = maxHP;
	}

	private void Update()
	{
		if (!_isGrinding) return;

		_grindTimer -= Time.deltaTime;
		if (_grindTimer <= 0f)
		{
			_grindTimer = grindInterval;
			GrindTick();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (isIndestructible) return;

		var player = collision.gameObject.GetComponent<PlayerController>();
		if (player == null) return;

		var vortex = collision.gameObject.GetComponentInChildren<VortexManager>();
		if (vortex == null) vortex = collision.gameObject.GetComponent<VortexManager>();
		if (vortex == null) return;

		StartGrinding(player, vortex);
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.GetComponent<PlayerController>() != null)
		{
			StopGrinding();
		}
	}

	private void StartGrinding(PlayerController player, VortexManager vortex)
	{
		_isGrinding = true;
		_grindTimer = grindInterval;
		_grindingVortex = vortex;
		_grindingPlayer = player;

		player.SetGrinding(true);
	}

	private void StopGrinding()
	{
		_isGrinding = false;
		_grindingVortex = null;

		if (_grindingPlayer != null)
		{
			_grindingPlayer.SetGrinding(false);
			_grindingPlayer = null;
		}
	}

	private void GrindTick()
	{
		if (_grindingVortex == null || _grindingVortex.Count == 0)
		{
			StopGrinding();
			if (GameManagerTT.Instance.ScrapCount <= 0)
				GameManagerTT.Instance.UpdateState(GameState.Fail);
			return;
		}

		ScrapItem consumed = _grindingVortex.RemoveScrap();
		if (consumed != null)
			Destroy(consumed.gameObject);

		TakeDamage(1);
		Haptics.Impact();
	}

	public void TakeDamage(int damage)
	{
		_currentHP -= damage;

		if (_currentHP <= 0)
		{
			Break();
		}
	}

	private void Break()
	{
		StopGrinding();

		AudioService.Instance?.PlayCollision();

		// Scatter child rigidbodies with explosion force
		var children = GetComponentsInChildren<Rigidbody>();
		foreach (var rb in children)
		{
			rb.isKinematic = false;
			rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
		}

		// If no child rigidbodies, just disable
		if (children.Length == 0)
		{
			gameObject.SetActive(false);
		}

		// Disable collider so player passes through
		var col = GetComponent<Collider>();
		if (col != null) col.enabled = false;

		Destroy(gameObject, 2f);
	}
}
