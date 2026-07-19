using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class InGameMessages : MonoBehaviour
    {
        public static InGameMessages Instance { get; private set; }

        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _animationTime = .5f;
        [SerializeField] private float _messageDuration = 3f;

        private string[] _messages;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        private void Start()
        {
            _messages = new[] {
                "Use your sonar to locate sunken relics and bring them to the surface.",
                "1/3",
                "2/3",
            };

            _text.SetText(string.Empty);
            StartCoroutine(HideAsync());
        }

        public void ShowMessage(int index)
        {
            index = Mathf.Clamp(index, 0, _messages.Length - 1);
            StartCoroutine(ShowAsync(_messages[index]));
        }

        private IEnumerator ShowAsync(string text, Action callback = null)
        {
            _text.SetText(text);

            yield return _text.transform.DOScale(Vector3.one, _animationTime)
                .SetUpdate(UpdateType.Normal, true)
                .SetEase(Ease.OutSine).WaitForCompletion();
            yield return new WaitForSeconds(_messageDuration);
            yield return HideAsync(callback);
        }

        private IEnumerator HideAsync(Action callback = null)
        {
            yield return _text.transform.transform.DOScale(Vector3.zero, _animationTime)
                .SetUpdate(UpdateType.Normal, true)
                .SetEase(Ease.InSine);
            callback?.Invoke();
        }
    }
}