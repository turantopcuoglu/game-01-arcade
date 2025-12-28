using UnityEngine;
using Project.Systems.Save;


namespace Project.Systems.Haptics
{
	public static class Haptics
	{
		public static bool Enabled = true;
		private const int CLICK_DURATION_MS = 35; // 30–50 arasý ayarlanýr

		public static void Click()
		{
			if (!SettingsData.Vibration) return;

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
                        CLICK_DURATION_MS,
                        vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE")
                    );
                    vibrator.Call("vibrate", effect);
                }
                else
                {
                    vibrator.Call("vibrate", CLICK_DURATION_MS);
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
