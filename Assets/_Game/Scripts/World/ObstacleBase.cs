using UnityEngine;

/// <summary>
/// Grindable obstacle — implements IDamageable.
/// When the player collides, grinding starts: scraps are consumed from the
/// vortex each tick, dealing 1 damage. At HP=0 the obstacle breaks apart.
///
/// Feedback (audio, haptics, camera shake) is NOT handled here.
/// ObstacleBase fires GameEvents, and FeedbackManager responds.
/// This keeps the class focused on gameplay logic only.
/// </summary>
public class ObstacleBase : MonoBehaviour, IDamageable
{
	[Header("Stats")]
	[SerializeField] private int maxHP = 2;
	[SerializeField] private bool isIndestructible = false;

	[Header("Grinding")]
	[SerializeField] private float grindInterval = 0.1f;

	[Header("Break Effect")]
	[SerializeField] private float explosionForce = 300f;
	[SerializeField] private float explosionRadius = 3f;
	[SerializeField] private GameObject piecesRoot;

	private int _currentHP;
	private bool _isGrinding;
	private float _grindTimer;
	private VortexManager _grindingVortex;
	private PlayerController _grindingPlayer;

	// ── IDamageable ────────────────────────────────────
	public int CurrentHP => _currentHP;
	public int MaxHP => maxHP;
	public bool IsAlive => _currentHP > 0;

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

	// ── Collision ──────────────────────────────────────

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

	// ── Grinding ───────────────────────────────────────

	private void StartGrinding(PlayerController player, VortexManager vortex)
	{
		_isGrinding = true;
		_grindTimer = grindInterval;
		_grindingVortex = vortex;
		_grindingPlayer = player;

		player.SetGrinding(true);
		GameEvents.GrindStarted();
	}

	private void StopGrinding()
	{
		if (!_isGrinding) return;

		_isGrinding = false;
		_grindingVortex = null;

		if (_grindingPlayer != null)
		{
			_grindingPlayer.SetGrinding(false);
			_grindingPlayer = null;
		}

		GameEvents.GrindStopped();
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
		GameEvents.GrindTick();
	}

	// ── IDamageable ────────────────────────────────────

	public void TakeDamage(int damage)
	{
		_currentHP -= damage;
		GameEvents.DamageTaken(this, _currentHP);

		if (_currentHP <= 0)
		{
			Break();
		}
	}

	// ── Break ──────────────────────────────────────────

	private void Break()
	{
		StopGrinding();

		// Notify via event bus (FeedbackManager handles audio/haptics/camera)
		GameEvents.ObstacleDestroyed(this);

		if (piecesRoot != null)
			piecesRoot.SetActive(true);

		// Scatter child rigidbodies with explosion force
		var children = GetComponentsInChildren<Rigidbody>();
		foreach (var rb in children)
		{
			rb.isKinematic = false;
			rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
		}

		if (children.Length == 0)
			gameObject.SetActive(false);

		var col = GetComponent<Collider>();
		if (col != null) col.enabled = false;

		var mesh = GetComponent<MeshRenderer>();
		if (mesh != null) mesh.enabled = false;

		Destroy(gameObject, 2f);
	}
}
