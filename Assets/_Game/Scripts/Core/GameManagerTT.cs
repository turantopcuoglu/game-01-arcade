using System;
using UnityEngine;

public class GameManagerTT : MonoBehaviour
{
    public static GameManagerTT Instance { get; private set; }

	public GameState CurrentState { get; private set; }

	public event Action<GameState> OnStateChanged;

	private int _scrapCount;
	public int ScrapCount
	{
		get => _scrapCount;
		private set
		{
			_scrapCount = Mathf.Max(0, value);
			OnScrapCountChanged?.Invoke(_scrapCount);
		}
	}

	public event Action<int> OnScrapCountChanged;

	private int _currentLevel;
	public int CurrentLevel => _currentLevel;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	private void Start()
	{
		_currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
		UpdateState(GameState.Menu);
	}

	public void UpdateState(GameState newState)
	{
		if (CurrentState == newState) return;

		CurrentState = newState;
		Debug.Log($"[GameManager] State -> {CurrentState}");

		switch (newState)
		{
			case GameState.Boot:
				break;
			case GameState.Menu:
				HandleMenu();
				break;
			case GameState.Gameplay:
				HandleGameplay();
				break;
			case GameState.Grinding:
				HandleGrinding();
				break;
			case GameState.Pause:
				HandlePause();
				break;
			case GameState.Finishing:
				HandleFinishing();
				break;
			case GameState.Win:
				HandleWin();
				break;
			case GameState.Fail:
				HandleFail();
				break;
		}

		OnStateChanged?.Invoke(newState);
	}

	private void HandleMenu()
	{
		Time.timeScale = 1f;
	}

	private void HandleGameplay()
	{
		Time.timeScale = 1f;
	}

	private void HandleGrinding()
	{
		// Speed reduction handled by PlayerController
	}

	private void HandlePause()
	{
		Time.timeScale = 0f;
	}

	private void HandleFinishing()
	{
		// Player reached finish line — scraps flying to titan.
		// Win panel is NOT shown yet. Transition to Win happens
		// after all scraps are delivered (triggered by FinishLine).
	}

	private void HandleWin()
	{
		_currentLevel++;
		PlayerPrefs.SetInt("CurrentLevel", _currentLevel);
		PlayerPrefs.Save();
	}

	private void HandleFail()
	{
	}

	// --- Soft Reset (no scene reload) ---

	/// <summary>
	/// Resets game state for retry / next-level without reloading the scene.
	/// Call while the screen is covered by PassingEffect.
	/// </summary>
	public void SoftReset()
	{
		_currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
		ScrapCount = 0;

		// Force state to Boot so UpdateState(Menu) won't be skipped
		// if we are already in Menu.
		CurrentState = GameState.Boot;
		UpdateState(GameState.Menu);
	}

	// --- Scrap API (called by VortexManager) ---

	public void InitScrapCount(int count)
	{
		ScrapCount = count;
	}

	public void AddScrap(int amount = 1)
	{
		ScrapCount += amount;
	}

	public bool TryConsumeScrap(int amount = 1)
	{
		if (ScrapCount < amount) return false;
		ScrapCount -= amount;
		return true;
	}
}

public enum GameState
{
	Boot,
	Menu,
	Gameplay,
	Grinding,
	Pause,
	Finishing,
	Win,
	Fail
}
