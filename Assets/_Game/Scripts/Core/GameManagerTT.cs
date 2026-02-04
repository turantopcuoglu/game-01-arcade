using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerTT : MonoBehaviour
{
    public static GameManagerTT Instance { get; private set; }

	public GameState CurrentState { get; private set; }

	public event Action<GameState> OnStateChanged;

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
		UpdateState(GameState.Menu);
	}

	public void UpdateState(GameState newState)
	{
		CurrentState = newState;
		Debug.Log($"State changed to: {CurrentState}");

		switch(newState)
		{
			case GameState.Boot:
				// Initialize game resources
				break;
			case GameState.Menu:
				HandleMenu();
				break;
			case GameState.Gameplay:
				// Start gameplay
				HandleGameplay();
				break;
			case GameState.Pause:
				// Pause the game
				HandlePause();
				break;
			case GameState.GameOver:
				// Handle game over logic
				HandleGameover();
				break;
		}

		OnStateChanged?.Invoke(newState);
	}

	private void HandleGameover()
	{
		Time.timeScale = 0f;
	}

	private void HandlePause()
	{
		Time.timeScale = 0f;
	}

	private void HandleGameplay()
	{
		Time.timeScale = 1f;
	}

	private void HandleMenu()
	{
		Time.timeScale = 1f;
	}
}

public enum GameState
{
	Boot,
	Menu,
	Gameplay,
	Pause,
	GameOver
}
