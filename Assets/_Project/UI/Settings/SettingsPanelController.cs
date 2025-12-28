using UnityEngine;
using UnityEngine.UI;
using Project.Systems.Save;

namespace Project.UI
{
	public class SettingsPanelController : MonoBehaviour
	{
		[Header("UI")]
		[SerializeField] private GameObject panelRoot;
		[SerializeField] private Toggle soundToggle;
		[SerializeField] private Toggle vibrationToggle;

		private bool _binding;

		private void OnEnable()
		{
			if (panelRoot != null) panelRoot.SetActive(false);

			BindFromData();

			if (soundToggle != null) soundToggle.onValueChanged.AddListener(OnSoundChanged);
			if (vibrationToggle != null) vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);
		}

		private void OnDisable()
		{
			if (soundToggle != null) soundToggle.onValueChanged.RemoveListener(OnSoundChanged);
			if (vibrationToggle != null) vibrationToggle.onValueChanged.RemoveListener(OnVibrationChanged);
		}

		public void Open()
		{
			BindFromData();
			if (panelRoot != null) panelRoot.SetActive(true);
		}

		public void Close()
		{
			if (panelRoot != null) panelRoot.SetActive(false);
		}

		private void BindFromData()
		{
			_binding = true;

			if (soundToggle != null) soundToggle.isOn = SettingsData.Sound;
			if (vibrationToggle != null) vibrationToggle.isOn = SettingsData.Vibration;

			_binding = false;
		}

		private void OnSoundChanged(bool value)
		{
			if (_binding) return;
			SettingsData.Sound = value;
		}

		private void OnVibrationChanged(bool value)
		{
			if (_binding) return;
			SettingsData.Vibration = value;
		}
	}
}
