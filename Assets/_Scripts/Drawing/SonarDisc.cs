using System;
using DG.Tweening;
using UnityEngine;

namespace Drawing
{
    public class SonarDisc
    {
        public Vector3 Position { get; private set; }

        public float CurrentSize { get; private set; }

        public bool Animating { get; private set; }

        private readonly float _maxSize;
        private readonly float _duration;

        public SonarDisc(Vector3 position, float maxSize, float duration)
        {
            Position = position;
            _maxSize = maxSize;
            _duration = duration;
        }

        public void Draw(Action onComplete, Vector3? positionOverride = null)
        {
            Position = positionOverride ?? Position;
            CurrentSize = 0f;
            Animating = true;
            
            DOTween.To(() => CurrentSize, x => CurrentSize = x, _maxSize, _duration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    Animating = false;
                    onComplete?.Invoke();
                });
        }
    }
}