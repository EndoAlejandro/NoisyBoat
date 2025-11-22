using System;
using DG.Tweening;
using UnityEngine;

namespace Drawing
{
    public class SonarDisc
    {
        public Vector3 Position { get; private set; }

        public Color Color { get; private set; }

        public float CurrentSize { get; private set; }

        public float CurrentAlpha { get; private set; }

        public bool IsAnimating { get; private set; }

        private readonly float _maxSize;
        private readonly float _duration;
        private readonly Color _defaultColor;

        public SonarDisc(Vector3 position, float maxSize, float duration, Color defaultColor)
        {
            Position = position;
            _maxSize = maxSize;
            _duration = duration;
            _defaultColor = defaultColor;
        }

        public void Draw(Color color, Vector3? positionOverride = null)
        {
            Color = color;
            Position = positionOverride ?? Position;
            CurrentSize = 0f;
            CurrentAlpha = 1f;
            IsAnimating = true;

            DOTween.To(() => CurrentAlpha, x => CurrentAlpha = x, 0f, _duration)
                .SetEase(Ease.OutSine);
            DOTween.To(() => CurrentSize, x => CurrentSize = x, _maxSize, _duration)
                .SetEase(Ease.OutSine)
                .OnComplete(() => IsAnimating = false);
        }
    }
}