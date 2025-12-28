using UnityEngine;
using Project.Systems.Save;

namespace Project.Audio
{
	public sealed class AudioService : MonoBehaviour
	{
		public static AudioService Instance { get; private set; }

		[Header("Clips")]
		[SerializeField] private AudioClip clickClip;

		private AudioSource _sfxSource;

		private void Awake()
		{
			if (Instance != null) { Destroy(gameObject); return; }
			Instance = this;
			DontDestroyOnLoad(gameObject);

			// SFX source her zaman garanti olsun
			_sfxSource = GetComponent<AudioSource>();
			if (_sfxSource == null) _sfxSource = gameObject.AddComponent<AudioSource>();

			_sfxSource.playOnAwake = false;
			_sfxSource.loop = false;
			_sfxSource.spatialBlend = 0f; // 2D
		}

		public void PlayOneShot(AudioClip clip, float volume = 1f)
		{
			if (!SettingsData.Sound) return;
			if (clip == null) return;

			_sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
			Debug.Log("Tap Audio played");
		}

		public void PlayClick(float volume = 0.7f)
		{
			PlayOneShot(clickClip, volume);
		}
	}
}
