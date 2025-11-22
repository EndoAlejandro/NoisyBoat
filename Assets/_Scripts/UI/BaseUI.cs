using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class BaseUI : MonoBehaviour
    {
        protected bool IsActive { get; private set; } = true;

        [SerializeField] protected Transform _container;

        [SerializeField] protected Button _leftButton;
        [SerializeField] protected Button _midButton;
        [SerializeField] protected Button _rightButton;


        [Space]
        [SerializeField] private float _time;

        [SerializeField, Range(0f, 1f)] private float _timingScale = .2f;


        private Coroutine _animation;
        private float _timer = 2f;

        private void Awake()
        {
            _leftButton.onClick.AddListener(LeftButtonOnClick);
            _midButton.onClick.AddListener(MidButtonOnClick);
            _rightButton.onClick.AddListener(RightButtonOnClick);
        }

        protected abstract void LeftButtonOnClick();
        protected abstract void MidButtonOnClick();
        protected abstract void RightButtonOnClick();

        [ContextMenu("Toggle")]
        private void Toggle()
        {
            if (IsActive) Hide();
            else Show();
        }

        protected void Show(Action callback = null)
        {
            if (_animation != null) StopCoroutine(_animation);
            _animation = StartCoroutine(ShowAsync(callback));
            IsActive = true;
        }

        protected void Hide(Action callback = null)
        {
            if (_animation != null) StopCoroutine(_animation);
            _animation = StartCoroutine(HideAsync(callback));
            IsActive = false;
        }

        private IEnumerator ShowAsync(Action callback)
        {
            yield return _container.DOScale(Vector3.one, _time).SetEase(Ease.OutSine).WaitForCompletion();

            yield return _leftButton.transform.DOScale(Vector3.one, _time).SetEase(Ease.OutSine);
            yield return new WaitForSeconds(_time * _timingScale);
            yield return _midButton.transform.DOScale(Vector3.one, _time).SetEase(Ease.OutSine);
            yield return new WaitForSeconds(_time * _timingScale);
            yield return _rightButton.transform.DOScale(Vector3.one, _time).SetEase(Ease.OutSine).WaitForCompletion();
            
            callback?.Invoke();
        }

        private IEnumerator HideAsync(Action callback)
        {
            yield return _leftButton.transform.DOScale(Vector3.zero, _time).SetEase(Ease.InSine);
            yield return new WaitForSeconds(_time * _timingScale);
            yield return _midButton.transform.DOScale(Vector3.zero, _time).SetEase(Ease.InSine);
            yield return new WaitForSeconds(_time * _timingScale);
            yield return _rightButton.transform.DOScale(Vector3.zero, _time).SetEase(Ease.InSine).WaitForCompletion();

            yield return _container.DOScale(Vector3.zero, _time).SetEase(Ease.InSine).WaitForCompletion();
            
            callback?.Invoke();
        }
    }
}