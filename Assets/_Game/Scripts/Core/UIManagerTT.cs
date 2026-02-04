using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerTT : MonoBehaviour
{
	[SerializeField] private Transform bootScreen;
	[SerializeField] private Transform menuPanel;
	[SerializeField] private Transform hudPanel;
	[SerializeField] private Transform pausePanel;
	[SerializeField] private Transform settingsPanel;
	[SerializeField] private Transform gameOverPanel;

	private Transform[] UIPanels;

	private void Awake()
	{
		UIPanels = new Transform[]
		{
			bootScreen,
			menuPanel,
			hudPanel,
			pausePanel,
			gameOverPanel
		};

		GameManagerTT.Instance.OnStateChanged += OnGameStateChanged;
	}
	private void Start()
	{
		
	}
	private void OnDestroy()
	{
		GameManagerTT.Instance.OnStateChanged -= OnGameStateChanged;
	}

	private void OnGameStateChanged(GameState state)
	{
		// Disable all UI panels
		foreach (Transform panel in UIPanels)
		{
			panel.gameObject.SetActive(false);
		}

		// Enable the relevant UI panel based on the game state
		switch (state)
		{
			case GameState.Boot:
				// Show boot screen
				bootScreen.gameObject.SetActive(true);
				break;
			case GameState.Menu:
				// Show menu UI
				menuPanel.gameObject.SetActive(true);
				break;
			case GameState.Gameplay:
				// Hide menu UI, show gameplay HUD
				hudPanel.gameObject.SetActive(true);
				break;
			case GameState.Pause:
				// Show pause menu
				pausePanel.gameObject.SetActive(true);
				break;
			case GameState.GameOver:
				// Show game over screen
				gameOverPanel.gameObject.SetActive(true);
				break;
		}
	}

	public void OnPlayButtonPressed()
	{
		Debug.Log("Play button pressed");
		GameManagerTT.Instance.UpdateState(GameState.Gameplay);
	}
}
