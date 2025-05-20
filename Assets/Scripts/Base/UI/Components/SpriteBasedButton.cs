using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Base.UI
{
    public class SpriteBasedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private float _holdScaleMultiplier = 0.9f;
        [SerializeField] private Color _holdColor = Color.white;
        [SerializeField] private SpriteRenderer _targetGraphic;
        [SerializeField] private float _animationSpeed = 5f;
        [SerializeField] private UnityEvent _onClickAction;

        private Color OriginalColor { get; set; }
        private Vector3 DefaultScale { get; set; }
        private Vector3 HoldScale { get; set; }
        private bool IsPointerDown { get; set; }

        private void Awake()
        {
            DefaultScale = transform.localScale;
            HoldScale = DefaultScale * _holdScaleMultiplier;
            OriginalColor = _targetGraphic.color;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsPointerDown)
            {
                _onClickAction?.Invoke();
            }

            IsPointerDown = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsPointerDown = false;
        }

        private void Update()
        {
            var targetScale = IsPointerDown ? HoldScale : DefaultScale;
            if (transform.localScale != targetScale)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, Time.deltaTime * _animationSpeed);
            }

            var targetColor = IsPointerDown ? _holdColor : OriginalColor;
            if (_targetGraphic.color != targetColor)
            {
                _targetGraphic.color = Color.Lerp(_targetGraphic.color, targetColor, Time.deltaTime * _animationSpeed * 2f);
            }
        }
    }
}