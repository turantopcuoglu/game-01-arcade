using UnityEngine;
using UnityEngine.UI;
using Project.Systems.Save;

namespace Project.UI
{
	/// <summary>
	/// Settings panel controller - handles sound and vibration toggles.
	/// This panel is an overlay and can be opened from any screen.
	/// </summary>
	public class SettingsPanelController : MonoBehaviour
	{
		[Header("Toggles")]
		[SerializeField] private Toggle soundToggle;
		[SerializeField] private Toggle vibrationToggle;

		private bool _binding;

		private void OnEnable()
		{
			BindFromData();

			if (soundToggle != null)
				soundToggle.onValueChanged.AddListener(OnSoundChanged);
			if (vibrationToggle != null)
				vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);
		}

		private void OnDisable()
		{
			if (soundToggle != null)
				soundToggle.onValueChanged.RemoveListener(OnSoundChanged);
			if (vibrationToggle != null)
				vibrationToggle.onValueChanged.RemoveListener(OnVibrationChanged);
		}

		private void BindFromData()
		{
			_binding = true;

			if (soundToggle != null)
				soundToggle.isOn = SettingsData.Sound;
			if (vibrationToggle != null)
				vibrationToggle.isOn = SettingsData.Vibration;

			_binding = false;
		}

		private void OnSoundChanged(bool value)
		{
			if (_binding) return;
			SettingsData.Sound = value;
			Debug.Log($"[Settings] Sound: {value}");
		}

		private void OnVibrationChanged(bool value)
		{
			if (_binding) return;
			SettingsData.Vibration = value;
			Debug.Log($"[Settings] Vibration: {value}");
		}

		// -------- UI Button Callbacks --------

		public void OnCloseClicked()
		{
			Debug.Log("[UI] Settings close clicked");
			UIManager.Instance?.OnSettingsCloseClicked();
		}
	}
}
