using UnityEngine;

namespace Project.Gameplay
{
	public class DifficultyController : MonoBehaviour
	{
		[Header("Progress")]
		[SerializeField] private float rampDuration = 90f; // 90 sn'de max'a yaklaþ

		[Header("Obstacle Speed")]
		[SerializeField] private float startSpeed = 6f;
		[SerializeField] private float endSpeed = 11f;

		[Header("Spawn Interval")]
		[SerializeField] private float startInterval = 1.2f;
		[SerializeField] private float endInterval = 0.65f;

		private float _t;

		public float CurrentSpeed { get; private set; }
		public float CurrentInterval { get; private set; }

		public void ResetDifficulty()
		{
			_t = 0f;
			Evaluate(0f);
		}

		public void Tick(float dt)
		{
			_t += dt;
			Evaluate(_t);
		}

		private void Evaluate(float time)
		{
			float k = Mathf.Clamp01(time / rampDuration);
			// daha doðal artýþ için ease
			k = Mathf.SmoothStep(0f, 1f, k);

			CurrentSpeed = Mathf.Lerp(startSpeed, endSpeed, k);
			CurrentInterval = Mathf.Lerp(startInterval, endInterval, k);
		}
	}
}