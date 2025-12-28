using UnityEngine;

namespace Project.Core
{
	public class Bootstrapper : MonoBehaviour
	{
		[SerializeField] private int targetFps = 60;

		private void Awake()
		{
			Application.targetFrameRate = targetFps;
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			//Project.Systems.Save.SettingsData.Sound = true;


			// Ýleride: init order, service locator / DI, analytics stub, save init vs.
			//Debug.Log($"[Bootstrapper] TargetFPS={targetFps}");
		}
	}
}
