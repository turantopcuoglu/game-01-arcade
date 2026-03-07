using UnityEngine;

/// <summary>
/// Manages level lifecycle: load, play, clear.
/// Reads LevelData ScriptableObjects and instantiates the correct level prefab.
/// </summary>
public class LevelManager : MonoBehaviour
{
	public static LevelManager Instance { get; private set; }

	[Header("Level Database")]
	[SerializeField] private LevelData[] levels;

	[Header("Scene References")]
	[SerializeField] private Transform levelParent;

	[SerializeField] private GameObject _LevelBindingsRoot;
	private GameObject _currentLevelInstance;

	public LevelData CurrentLevelData { get; private set; }

	public event System.Action<LevelData> OnLevelLoaded;
	public event System.Action OnLevelCleared;

	private void Awake()
	{
		if (Instance != null) { Destroy(gameObject); return; }
		Instance = this;
	}

	private void Start()
	{
		int levelIndex = GameManagerTT.Instance.CurrentLevel - 1;
		LoadLevel(levelIndex);
	}

	public void LoadLevel(int index)
	{
		ClearLevel();

		// Loop levels if we run out (endless replay)
		int safeIndex = index % levels.Length;
		if (safeIndex < 0 || levels.Length == 0) return;

		CurrentLevelData = levels[safeIndex];

		if (CurrentLevelData.levelPrefab != null)
		{
			_currentLevelInstance = Instantiate(
				CurrentLevelData.levelPrefab,
				Vector3.zero,
				Quaternion.identity,
				levelParent
			);

			// Auto-resolve external dependencies for prefab components
			var bindables = _currentLevelInstance.GetComponentsInChildren<IAutoBindable>();
			foreach (var bindable in bindables)
				bindable.AutoBind(_LevelBindingsRoot);
		}

		OnLevelLoaded?.Invoke(CurrentLevelData);
		GameEvents.LevelLoaded(CurrentLevelData);
	}

	public void ReloadCurrentLevel()
	{
		int levelIndex = GameManagerTT.Instance.CurrentLevel - 1;
		LoadLevel(levelIndex);
	}

	public void ClearLevel()
	{
		if (_currentLevelInstance != null)
		{
			// Reset camera priorities synchronously before Destroy.
			// Destroy is deferred to end-of-frame, so relying on OnDestroy
			// would let the new level's AutoBind read a dirty titanCam priority.
			var finishLine = _currentLevelInstance.GetComponentInChildren<FinishLine>();
			if (finishLine != null)
				finishLine.ResetCamera();

			Destroy(_currentLevelInstance);
			_currentLevelInstance = null;
		}

		OnLevelCleared?.Invoke();
	}
}
