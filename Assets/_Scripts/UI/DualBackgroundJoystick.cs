using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

namespace UI
{
    public class DualBackgroundJoystick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [InputControl(layout = "Vector2")]
        [SerializeField] private string m_ControlPath;

        [Header("Visual Elements")]
        [Tooltip("The background that snaps to the exact touch position")]
        [SerializeField] private RectTransform _dynamicBackground;

        [Tooltip("The thumbstick that moves based on drag")]
        [SerializeField] private RectTransform _knob;

        [Header("Settings")]
        [SerializeField] private float _movementRange = 50f;

        private Vector2 _startPos;
        private Vector2 _pointerDownPos;

        private Image _backgroundRenderer;
        private Image _knobRenderer;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }

        private void Awake()
        {
            _backgroundRenderer = _dynamicBackground.GetComponent<Image>();
            _knobRenderer = _knob.GetComponent<Image>();
        }

        private void Start()
        {
            // Save the default position to snap back to on release
            if (_dynamicBackground != null) _startPos = _dynamicBackground.anchoredPosition;
            
            SetJoystickVisibility(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null) return;

            // Get the touch position local to the invisible touch zone
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform,
                eventData.position,
                eventData.pressEventCamera,
                out _pointerDownPos
            );

            // Snap both the dynamic background and knob exactly to where the user touched
            if (_dynamicBackground != null) _dynamicBackground.anchoredPosition = _pointerDownPos;
            if (_knob != null) _knob.anchoredPosition = _pointerDownPos;
            
            SetJoystickVisibility(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 currentPosition
            );

            // Calculate drag distance from the original touch point
            Vector2 delta = currentPosition - _pointerDownPos;
            delta = Vector2.ClampMagnitude(delta, _movementRange);

            // Move ONLY the knob
            if (_knob != null) _knob.anchoredPosition = _pointerDownPos + delta;

            // Send standardized values (-1 to 1) to Unity's Input System
            SendValueToControl(delta / _movementRange);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Reset dynamic elements to their original positions
            if (_dynamicBackground != null) _dynamicBackground.anchoredPosition = _startPos;
            if (_knob != null) _knob.anchoredPosition = _startPos;

            // Stop movement
            SendValueToControl(Vector2.zero);
            SetJoystickVisibility(false);
        }

        private void SetJoystickVisibility(bool isVisible)
        {
            _backgroundRenderer.enabled = isVisible;
            _knobRenderer.enabled = isVisible;
        }
    }
}