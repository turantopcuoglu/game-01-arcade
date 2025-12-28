using UnityEngine;

namespace Project.Systems.Save
{
	public static class SettingsData
	{
		private const string SOUND = "settings_sound";
		private const string VIBRATION = "settings_vibration";

		public static bool Sound
		{
			get => PlayerPrefs.GetInt(SOUND, 1) == 1;
			set { PlayerPrefs.SetInt(SOUND, value ? 1 : 0); PlayerPrefs.Save(); }
		}

		public static bool Vibration
		{
			get => PlayerPrefs.GetInt(VIBRATION, 1) == 1;
			set { PlayerPrefs.SetInt(VIBRATION, value ? 1 : 0); PlayerPrefs.Save(); }
		}
	}
}
