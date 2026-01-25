using UnityEngine;
using System;

namespace Project.Gameplay
{
	/// <summary>
	/// Manages coin combo system.
	/// Combo increases when collecting coins without breaks.
	/// Higher combo = higher coin value.
	/// </summary>
	public class ComboSystem : MonoBehaviour
	{
		public static ComboSystem Instance { get; private set; }

		[Header("Combo Settings")]
		[SerializeField] private float comboTimeout = 2.5f;  // Seconds without coin before combo breaks
		[SerializeField] private int maxCombo = 10;          // Combo caps here

		[Header("Multiplier Tiers")]
		[SerializeField] private int tier1Threshold = 3;     // x1-x3: value = 1
		[SerializeField] private int tier2Threshold = 6;     // x4-x6: value = 2
		[SerializeField] private int tier3Threshold = 9;     // x7-x9: value = 3
		                                                      // x10+:  value = 4

		public int CurrentCombo { get; private set; }
		public int MaxComboThisRun { get; private set; }
		public int CoinValueMultiplier => GetCoinValue();
		public float ComboTimeRemaining => Mathf.Max(0f, comboTimeout - _timeSinceLastCoin);
		public float ComboTimeRatio => Mathf.Clamp01(1f - (_timeSinceLastCoin / comboTimeout));

		// Events for UI/VFX
		public event Action<int> OnComboChanged;          // Fires when combo changes
		public event Action<int> OnComboBroken;           // Fires when combo breaks (passes lost combo)
		public event Action<int> OnComboMilestone;        // Fires at x5, x10 milestones

		private float _timeSinceLastCoin;
		private bool _comboActive;

		private void Awake()
		{
			if (Instance != null) { Destroy(gameObject); return; }
			Instance = this;
		}

		private void Update()
		{
			if (!_comboActive || CurrentCombo == 0) return;

			_timeSinceLastCoin += Time.deltaTime;

			if (_timeSinceLastCoin >= comboTimeout)
			{
				BreakCombo();
			}
		}

		/// <summary>
		/// Call this when player collects a coin.
		/// Returns the coin value based on current combo.
		/// </summary>
		public int RegisterCoinPickup()
		{
			int coinValue = GetCoinValue();

			// Increment combo
			int oldCombo = CurrentCombo;
			CurrentCombo = Mathf.Min(CurrentCombo + 1, maxCombo);
			_timeSinceLastCoin = 0f;
			_comboActive = true;

			// Track max combo
			if (CurrentCombo > MaxComboThisRun)
				MaxComboThisRun = CurrentCombo;

			// Fire events
			if (CurrentCombo != oldCombo)
			{
				OnComboChanged?.Invoke(CurrentCombo);

				// Milestone events at x5, x10
				if (CurrentCombo == 5 || CurrentCombo == 10)
				{
					OnComboMilestone?.Invoke(CurrentCombo);
				}
			}

			return coinValue;
		}

		/// <summary>
		/// Manually break the combo (e.g., player missed a coin in trail).
		/// </summary>
		public void BreakCombo()
		{
			if (CurrentCombo > 0)
			{
				int lostCombo = CurrentCombo;
				CurrentCombo = 0;
				_comboActive = false;
				_timeSinceLastCoin = 0f;

				OnComboBroken?.Invoke(lostCombo);
				OnComboChanged?.Invoke(0);
			}
		}

		/// <summary>
		/// Reset combo system for new run.
		/// </summary>
		public void ResetCombo()
		{
			CurrentCombo = 0;
			MaxComboThisRun = 0;
			_timeSinceLastCoin = 0f;
			_comboActive = false;
			OnComboChanged?.Invoke(0);
		}

		/// <summary>
		/// Get coin value based on current combo tier.
		/// </summary>
		private int GetCoinValue()
		{
			if (CurrentCombo < tier1Threshold)
				return 1;
			if (CurrentCombo < tier2Threshold)
				return 2;
			if (CurrentCombo < tier3Threshold)
				return 3;
			return 4;
		}

		/// <summary>
		/// Get combo tier for visual feedback (0-3).
		/// </summary>
		public int GetComboTier()
		{
			if (CurrentCombo < tier1Threshold) return 0;
			if (CurrentCombo < tier2Threshold) return 1;
			if (CurrentCombo < tier3Threshold) return 2;
			return 3;
		}
	}
}
