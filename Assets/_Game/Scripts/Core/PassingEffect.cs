using System;
using System.Collections;
using UnityEngine;

public class PassingEffect : MonoBehaviour
{
	public static PassingEffect Instance { get; private set; }
	private bool _isPlaying;

	[SerializeField] private Animator transactionAnimator;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public void Play(Action onScreenCovered = null)
	{
		if (_isPlaying) return;
		transactionAnimator.SetTrigger("StartTransition");
		Invoke("OnTransitionStart", 0.1f); // Delay to ensure the animation starts before setting the flag
		Invoke("OnTransitionEnd", 1.5f); // Adjust this delay based on your animation length)
	}

	public void PlayWithSceneReload()
	{
		if (_isPlaying) return;
		transactionAnimator.SetTrigger("StartTransition");
		Invoke("OnTransitionStart", 0.1f); // Delay to ensure the animation starts before setting the flag
		Invoke("OnTransitionEnd", 1.5f); // Adjust this delay based on your animation length)

	}

	private void OnTransitionStart()
	{
		_isPlaying = true;
	}

	private void OnTransitionEnd()
	{
		_isPlaying = false;
	}

	public bool IsPlaying => _isPlaying;
	
}
