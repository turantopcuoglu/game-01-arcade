using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.UI
{
	public class ClickPunch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField] private float pressedScale = 0.92f;
		[SerializeField] private float lerpSpeed = 18f;

		private Vector3 _defaultScale;
		private bool _pressed;

		private void Awake() => _defaultScale = transform.localScale;

		public void OnPointerDown(PointerEventData eventData) => _pressed = true;
		public void OnPointerUp(PointerEventData eventData) => _pressed = false;

		private void Update()
		{
			var target = _pressed ? _defaultScale * pressedScale : _defaultScale;
			transform.localScale = Vector3.Lerp(transform.localScale, target, Time.unscaledDeltaTime * lerpSpeed);
		}

		private void OnDisable()
		{
			transform.localScale = _defaultScale;
			_pressed = false;
		}
	}
}
