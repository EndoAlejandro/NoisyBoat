using System;
using DG.Tweening;
using UnityEngine;

public class TargetWaves
{
    public Vector3 Position { get; private set; }

    public float CurrentSize { get; private set; }

    public float CurrentAlpha { get; private set; }

    public bool IsAnimating { get; private set; }


    private readonly float _maxSize;
    private readonly float _duration;

    public TargetWaves(Vector3 position, float maxSize, float duration)
    {
        Position = position;
        _maxSize = maxSize;
        _duration = duration;
    }

    public TargetWaves(float maxSize, float duration) : this(Vector3.zero, maxSize, duration) { }

    public void Draw(Action onComplete)
    {
        CurrentSize = 0f;
        CurrentAlpha = 1f;
        IsAnimating = true;
        DOTween.To(() => CurrentAlpha, x => CurrentAlpha = x, 0f, _duration)
            .SetEase(Ease.OutSine);

        DOTween.To(() => CurrentSize, x => CurrentSize = x, _maxSize, _duration)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                IsAnimating = false;
                onComplete?.Invoke();
            });
    }
}