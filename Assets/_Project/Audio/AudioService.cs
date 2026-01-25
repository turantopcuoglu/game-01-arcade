using UnityEngine;
using Project.Systems.Save;

namespace Project.Audio
{
	public sealed class AudioService : MonoBehaviour
	{
		public static AudioService Instance { get; private set; }

		[Header("Clips")]
		[SerializeField] private AudioClip clickClip;
		[SerializeField] private AudioClip coinPickupClip;
		[SerializeField] private AudioClip collisionClip;

		private AudioSource _sfxSource;

		private void Awake()
		{
			if (Instance != null) { Destroy(gameObject); return; }
			Instance = this;
			DontDestroyOnLoad(gameObject);

			_sfxSource = GetComponent<AudioSource>();
			if (_sfxSource == null) _sfxSource = gameObject.AddComponent<AudioSource>();

			_sfxSource.playOnAwake = false;
			_sfxSource.loop = false;
			_sfxSource.spatialBlend = 0f;
		}

		public void PlayOneShot(AudioClip clip, float volume = 1f)
		{
			if (!SettingsData.Sound) return;
			if (clip == null) return;

			_sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
		}

		public void PlayClick(float volume = 0.7f)
		{
			PlayOneShot(clickClip, volume);
		}

		public void PlayCoinPickup(float volume = 0.6f)
		{
			PlayOneShot(coinPickupClip, volume);
		}

		public void PlayCollision(float volume = 0.8f)
		{
			PlayOneShot(collisionClip, volume);
		}
	}
}
