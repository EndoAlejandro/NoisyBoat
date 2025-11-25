using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] private Image _image;
    [SerializeField] private float _time = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        Hide();
    }

    public void Show(Action callback = null)
    {
        var color = _image.color;
        _image.gameObject.SetActive(true);

        DOTween.To(() => color.a, x =>
            {
                color.a = x;
                _image.color = color;
            }, 1f, _time)
            .SetEase(Ease.OutSine)
            .SetUpdate(UpdateType.Normal, true)
            .OnComplete(() => callback?.Invoke());
    }

    public void Hide(Action callback = null)
    {
        var color = _image.color;

        DOTween.To(() => color.a, x =>
            {
                color.a = x;
                _image.color = color;
            }, 0f, _time)
            .SetEase(Ease.OutSine)
            .SetUpdate(UpdateType.Normal, true)
            .OnComplete(() =>
            {
                _image.gameObject.SetActive(false);
                callback?.Invoke();
            });
    }
}