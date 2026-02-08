using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Systems.Input
{
	public sealed class InputRouter : MonoBehaviour
	{
		public static InputRouter Instance { get; private set; }

		private PlayerInputActions _actions;

		public event Action<Vector2> OnTapScreen;
		public event Action<Vector2> OnPointerScreen;
		public event Action<Vector2> OnDragDelta;
		public event Action OnFingerUp;

		public event Action OnTwoFingerTap;

		private float _twoFingerCooldown;


		private void Awake()
		{
			if (Instance != null) { Destroy(gameObject); return; }
			Instance = this;
			DontDestroyOnLoad(gameObject);

			_actions = new PlayerInputActions();
		}

		private void OnEnable()
		{
			_actions.Enable();

			_actions.Player.Tap.performed += HandleTap;
			_actions.Player.Tap.canceled += HandleFingerUp;
			_actions.Player.Point.performed += HandlePoint;
			_actions.Player.Drag.performed += HandleDrag;
		}


		private void Update()
		{
#if UNITY_EDITOR
			// Editor h�zl� test: ESC = pause
			if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
				OnTwoFingerTap?.Invoke();
#endif

#if UNITY_ANDROID || UNITY_IOS
			// Touchscreen yoksa ��k
			if (Touchscreen.current == null) return;

			// cooldown (ayn� frame spam �nlemek i�in)
			if (_twoFingerCooldown > 0f)
			{
				_twoFingerCooldown -= Time.unscaledDeltaTime;
				return;
			}

			// iki parmak dokunu� kontrol�
			var touches = Touchscreen.current.touches;
			int pressedCount = 0;

			for (int i = 0; i < touches.Count; i++)
			{
				var t = touches[i];
				if (t.press.isPressed) pressedCount++;
			}

			if (pressedCount >= 2)
			{
				OnTwoFingerTap?.Invoke();
				_twoFingerCooldown = 0.35f;
			}
#endif
		}

		private void HandleTap(InputAction.CallbackContext ctx)
		{
			var pos = _actions.Player.Point.ReadValue<Vector2>();
			OnTapScreen?.Invoke(pos);
		}

		private void HandleFingerUp(InputAction.CallbackContext ctx)
		{
			OnFingerUp?.Invoke();
		}


		private void HandlePoint(InputAction.CallbackContext ctx)
		{
			OnPointerScreen?.Invoke(ctx.ReadValue<Vector2>());
		}

		private void HandleDrag(InputAction.CallbackContext ctx)
		{
			var delta = ctx.ReadValue<Vector2>();
			//Debug.Log($"[InputRouter] Drag delta={delta}");
			OnDragDelta?.Invoke(delta);
		}

	}
}