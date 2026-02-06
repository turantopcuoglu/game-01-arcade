using UnityEngine;
using TMPro;

public class UIManagerTT : MonoBehaviour
{
	[Header("Panels")]
	[SerializeField] private Transform bootScreen;
	[SerializeField] private Transform menuPanel;
	[SerializeField] private Transform hudPanel;
	[SerializeField] private Transform pausePanel;
	[SerializeField] private Transform winPanel;
	[SerializeField] private Transform failPanel;

	[Header("HUD Elements")]
	[SerializeField] private TMP_Text scrapCountText;
	[SerializeField] private TMP_Text levelText;

	private Transform[] _allPanels;

	private void Awake()
	{
		_allPanels = new Transform[]
		{
			bootScreen,
			menuPanel,
			hudPanel,
			pausePanel,
			winPanel,
			failPanel
		};

		GameManagerTT.Instance.OnStateChanged += OnGameStateChanged;
		GameManagerTT.Instance.OnScrapCountChanged += OnScrapCountChanged;
	}

	private void Start()
	{
		if (levelText != null)
			levelText.text = $"Level {GameManagerTT.Instance.CurrentLevel}";
	}

	private void OnDestroy()
	{
		if (GameManagerTT.Instance == null) return;
		GameManagerTT.Instance.OnStateChanged -= OnGameStateChanged;
		GameManagerTT.Instance.OnScrapCountChanged -= OnScrapCountChanged;
	}

	private void OnGameStateChanged(GameState state)
	{
		foreach (Transform panel in _allPanels)
		{
			if (panel != null)
				panel.gameObject.SetActive(false);
		}

		switch (state)
		{
			case GameState.Boot:
				SetPanel(bootScreen);
				break;
			case GameState.Menu:
				SetPanel(menuPanel);
				break;
			case GameState.Gameplay:
			case GameState.Grinding:
				SetPanel(hudPanel);
				break;
			case GameState.Pause:
				SetPanel(pausePanel);
				break;
			case GameState.Win:
				SetPanel(winPanel);
				break;
			case GameState.Fail:
				SetPanel(failPanel);
				break;
		}
	}

	private void SetPanel(Transform panel)
	{
		if (panel != null)
			panel.gameObject.SetActive(true);
	}

	private void OnScrapCountChanged(int count)
	{
		if (scrapCountText != null)
			scrapCountText.text = count.ToString();
	}

	// --- Button Callbacks ---

	public void OnPlayButtonPressed()
	{
		GameManagerTT.Instance.UpdateState(GameState.Gameplay);
	}

	public void OnResumeButtonPressed()
	{
		GameManagerTT.Instance.UpdateState(GameState.Gameplay);
	}

	public void OnRetryButtonPressed()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(
			UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
	}

	public void OnNextLevelButtonPressed()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(
			UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
	}
}
