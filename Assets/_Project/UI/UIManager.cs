using System;
using UnityEngine;
using Project.Core;

namespace Project.UI
{
    /// <summary>
    /// Centralized UI panel manager.
    /// Listens to GameManager state changes and shows/hides panels accordingly.
    /// Reusable across different games - just assign panels in Inspector.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject gameplayHUD;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject settingsPanel;

        // Event for UI state changes (useful for animations, sounds, etc.)
        public event Action<GameStateId> OnPanelChanged;

        // Track if settings is open (overlay - can be open on top of other panels)
        public bool IsSettingsOpen => settingsPanel != null && settingsPanel.activeSelf;

        private GameStateId _lastState = GameStateId.None;

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
            // Initialize all panels to hidden
            HideAllPanels();
        }

        private void Update()
        {
            // Listen to GameManager state and update panels
            if (GameManager.Instance == null) return;

            var currentState = GameManager.Instance.CurrentState;
            if (currentState != _lastState)
            {
                _lastState = currentState;
                RefreshPanels(currentState);
            }
        }

        /// <summary>
        /// Shows/hides panels based on current game state.
        /// </summary>
        private void RefreshPanels(GameStateId state)
        {
            // Hide all main panels first (settings is overlay, handled separately)
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(gameplayHUD, false);
            SetPanelActive(pausePanel, false);
            SetPanelActive(gameOverPanel, false);

            // Show appropriate panel for state
            switch (state)
            {
                case GameStateId.Menu:
                    SetPanelActive(mainMenuPanel, true);
                    CloseSettings(); // Close settings when returning to menu
                    break;

                case GameStateId.Gameplay:
                    SetPanelActive(gameplayHUD, true);
                    CloseSettings(); // Close settings when gameplay resumes
                    break;

                case GameStateId.Pause:
                    SetPanelActive(gameplayHUD, true); // Keep HUD visible during pause
                    SetPanelActive(pausePanel, true);
                    break;

                case GameStateId.GameOver:
                    SetPanelActive(gameplayHUD, true); // Keep HUD visible on game over
                    SetPanelActive(gameOverPanel, true);
                    CloseSettings();
                    break;
            }

            OnPanelChanged?.Invoke(state);
            Debug.Log($"[UIManager] Panel changed to: {state}");
        }

        private void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null && panel.activeSelf != active)
            {
                panel.SetActive(active);
            }
        }

        private void HideAllPanels()
        {
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(gameplayHUD, false);
            SetPanelActive(pausePanel, false);
            SetPanelActive(gameOverPanel, false);
            SetPanelActive(settingsPanel, false);
        }

        // -------- Settings Panel (Overlay) --------

        /// <summary>
        /// Opens settings panel. Can be called from any screen.
        /// Pauses game if in Gameplay state.
        /// </summary>
        public void OpenSettings()
        {
            if (settingsPanel == null) return;

            // If we're in gameplay, pause first
            var gm = GameManager.Instance;
            if (gm != null && gm.CurrentState == GameStateId.Gameplay)
            {
                gm.Pause();
            }

            settingsPanel.SetActive(true);
            Debug.Log("[UIManager] Settings opened");
        }

        /// <summary>
        /// Closes settings panel.
        /// </summary>
        public void CloseSettings()
        {
            if (settingsPanel == null) return;
            settingsPanel.SetActive(false);
        }

        /// <summary>
        /// Toggles settings panel.
        /// </summary>
        public void ToggleSettings()
        {
            if (IsSettingsOpen)
                CloseSettings();
            else
                OpenSettings();
        }

        // -------- Button Callbacks (can be assigned in Inspector) --------

        public void OnStartGameClicked()
        {
            GameManager.Instance?.StartGame();
        }

        public void OnPauseClicked()
        {
            GameManager.Instance?.Pause();
        }

        public void OnResumeClicked()
        {
            CloseSettings();
            GameManager.Instance?.Resume();
        }

        public void OnRestartClicked()
        {
            CloseSettings();
            GameManager.Instance?.RestartRun();
        }

        public void OnMainMenuClicked()
        {
            CloseSettings();
            GameManager.Instance?.OpenMenu();
        }

        public void OnSettingsClicked()
        {
            OpenSettings();
        }

        public void OnSettingsCloseClicked()
        {
            CloseSettings();
        }
    }
}
