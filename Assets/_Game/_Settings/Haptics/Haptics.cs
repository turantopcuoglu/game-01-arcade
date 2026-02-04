using UnityEngine;
//using Project.Systems.Save;


namespace Project.Systems.Haptics
{
	public static class Haptics
	{
		public static bool Enabled = true;
		private const int CLICK_DURATION_MS = 35;
		private const int COIN_DURATION_MS = 20;
		private const int IMPACT_DURATION_MS = 50;

		public static void Click() => Vibrate(CLICK_DURATION_MS);

		public static void CoinPickup() => Vibrate(COIN_DURATION_MS);

		public static void Impact() => Vibrate(IMPACT_DURATION_MS);

		private static void Vibrate(int durationMs)
		{
			//if (!SettingsData.Vibration) return;

#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                using var context = activity.Call<AndroidJavaObject>("getApplicationContext");
                using var vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator");
                if (vibrator == null) return;

                if (AndroidVersion() >= 26)
                {
                    using var vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                    var effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                        "createOneShot",
                        durationMs,
                        vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE")
                    );
                    vibrator.Call("vibrate", effect);
                }
                else
                {
                    vibrator.Call("vibrate", durationMs);
                }
            }
            catch { }
#endif
		}

		private static int AndroidVersion()
		{
			using var version = new AndroidJavaClass("android.os.Build$VERSION");
			return version.GetStatic<int>("SDK_INT");
		}
	}
}
