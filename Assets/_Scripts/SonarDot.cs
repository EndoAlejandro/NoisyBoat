using DG.Tweening;
using UnityEngine;

public class SonarDot
{
    public Vector3 Position { get; private set; }

    public Vector3 Direction { get; private set; }

    public float CurrentSize { get; private set; }

    private readonly float _maxSize;
    private readonly float _increaseDuration;
    private readonly float _decreaseSpeed;

    private bool _canDecrease;

    public SonarDot(Vector3 position, float maxSize, float increaseDuration, float decreaseSpeed)
    {
        Position = position;
        _maxSize = maxSize;
        _increaseDuration = increaseDuration;
        _decreaseSpeed = decreaseSpeed;

        Direction = position.normalized;
    }

    public void Tick()
    {
        if (!_canDecrease || CurrentSize <= 0f) return;
        CurrentSize -= Time.deltaTime * _decreaseSpeed;
    }

    public void Increase(float normalizedValue)
    {
        _canDecrease = false;
        DOTween.To(() => CurrentSize, x => CurrentSize = x, normalizedValue * _maxSize, _increaseDuration)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => _canDecrease = true);
    }
}