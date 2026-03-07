using System;
using System.Collections;
using UnityEngine;

public class PassingEffect : MonoBehaviour
{
	public static PassingEffect Instance { get; private set; }
	private bool _isPlaying;
	private Action _onScreenCovered;

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
		_onScreenCovered = onScreenCovered;
		transactionAnimator.SetTrigger("StartTransition");
		Invoke("OnTransitionStart", 0.1f);
		Invoke("OnTransitionEnd", 1.8f);
	}

	public void PlayWithSceneReload()
	{
		if (_isPlaying) return;
		transactionAnimator.SetTrigger("StartTransition");
		Invoke("OnTransitionStart", 0.1f);
		Invoke("OnTransitionEnd", 1.8f);
	}

	private void OnTransitionStart()
	{
		_isPlaying = true;
	}

	private void OnTransitionEnd()
	{
		_isPlaying = false;
		_onScreenCovered?.Invoke();
		_onScreenCovered = null;
	}

	public bool IsPlaying => _isPlaying;

}
