using System.Collections;
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

	[Header("Menu Settings")]
	[SerializeField][Range(0f, 1f)] private float menuAlpha = 0.4f;
	[SerializeField] private float playDelay = 1.5f;

	[Header("HUD Elements")]
	[SerializeField] private TMP_Text scrapCountText;
	[SerializeField] private TMP_Text levelText;

	private Transform[] _allPanels;
	private CanvasGroup _menuCanvasGroup;

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

		if (menuPanel != null)
		{
			_menuCanvasGroup = menuPanel.GetComponent<CanvasGroup>();
			if (_menuCanvasGroup == null)
				_menuCanvasGroup = menuPanel.gameObject.AddComponent<CanvasGroup>();
		}

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
				if (_menuCanvasGroup != null)
					_menuCanvasGroup.alpha = menuAlpha;
				if (levelText != null)
					levelText.text = $"Level {GameManagerTT.Instance.CurrentLevel}";
				break;
			case GameState.Gameplay:
			case GameState.Grinding:
			case GameState.Finishing:
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
		if (menuPanel != null)
			menuPanel.gameObject.SetActive(false);

		StartCoroutine(StartGameplayAfterDelay());
	}

	private IEnumerator StartGameplayAfterDelay()
	{
		yield return new WaitForSeconds(playDelay);
		GameManagerTT.Instance.UpdateState(GameState.Gameplay);
	}

	public void OnResumeButtonPressed()
	{
		GameManagerTT.Instance.UpdateState(GameState.Gameplay);
	}

	public void OnRetryButtonPressed()
	{
		if (PassingEffect.Instance != null)
		{
			PassingEffect.Instance.Play(() =>
			{
				GameManagerTT.Instance.RestartLevel();
			});
		}
		else
		{
			GameManagerTT.Instance.RestartLevel();
		}
	}

	public void OnNextLevelButtonPressed()
	{
		if (PassingEffect.Instance != null)
		{
			PassingEffect.Instance.PlayWithSceneReload();
		}
		else
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(
				UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
		}
	}
}
