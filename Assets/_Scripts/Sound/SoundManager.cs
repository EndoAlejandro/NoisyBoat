using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        [Header("Heart Beat")]
        [SerializeField] private AudioSource _heartBeatAudioSource;

        [SerializeField] private float _heartChaseTransitionTime = 2f;

        [Space]
        [SerializeField] private float _heartCalmVolume = .5f;

        [SerializeField] private float _heartCalmPitch = .7f;

        [Space]
        [SerializeField] private float _heartChaseVolume = 1f;

        [SerializeField] private float _heartChasePitch = 1.5f;

        [Header("Wind")]
        [SerializeField] private AudioSource _windAudioSource;

        [SerializeField] private float _windChaseTransitionTime = 1f;

        [Space]
        [SerializeField] private float _windChaseVolume = .2f;

        [SerializeField] private float _windCalmVolume = 1f;

        private bool _isChasing;

        private void Start()
        {
            SetChaseState(false);
        }

        [ContextMenu("Toggle Audio")]
        private void ToggleAudio() => SetChaseState(!_isChasing);

        private void SetChaseState(bool isChasing)
        {
            _isChasing = isChasing;

            // Heart
            _heartBeatAudioSource
                .DOPitch(_isChasing ? _heartChasePitch : _heartCalmPitch, _heartChaseTransitionTime)
                .SetEase(Ease.InOutSine);
            _heartBeatAudioSource
                .DOFade(_isChasing ? _heartChaseVolume : _heartCalmVolume, _heartChaseTransitionTime)
                .SetEase(Ease.InOutSine);

            // Wind
            _windAudioSource
                .DOFade(_isChasing ? _windChaseVolume : _windCalmVolume, _windChaseTransitionTime)
                .SetEase(Ease.InOutSine);
        }
    }
}