using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Behaviours
{
    public class ScaleTransformBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler, IDisposable
    {
        [SerializeField] private float _scaleValue = 1.1f;
        [SerializeField] private float _animationDuration = .2f;

        private Selectable _selectable;
        private Tween _tween;
        private bool _isDisposed;

        private void Awake() => _selectable = GetComponent<Selectable>();

        private void OnDestroy() => StopTween();

        public void Dispose() => StopTween();

        private void StopTween()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            transform.localScale = Vector3.one;
            _tween?.Kill();
            _tween = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_selectable != null)
            {
                if (_selectable.interactable)
                    ScaleUp();
                return;
            }

            ScaleUp();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_selectable == null || _selectable.interactable)
                ScaleDown();
        }

        public void OnPointerClick(PointerEventData eventData) => ScaleDown();

        private void ScaleUp()
        {
            _tween = transform.DOScale(Vector3.one * _scaleValue, _animationDuration);
        }

        private void ScaleDown()
        {
            _tween = transform.DOScale(Vector3.one, _animationDuration);
        }
    }
}