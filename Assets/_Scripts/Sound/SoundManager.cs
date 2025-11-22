using DG.Tweening;
using Enemies;
using PlayerComponents;
using UnityEngine;
using UnityEngine.Audio;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioMixerSnapshot _baseSnapshot;
        [SerializeField] private AudioMixerSnapshot _telegraphSnapshot;
        [SerializeField] private AudioMixerSnapshot _chaseSnapshot;

        [Header("Heart Beat")]
        [SerializeField] private AudioSource _heartBeatAudioSource;

        [SerializeField] private float _heartChaseTransitionTime = 2f;

        [Space]
        [SerializeField] private float _heartCalmVolume = .75f;

        [SerializeField] private float _heartCalmPitch = 1f;

        [Space]
        [SerializeField] private float _heartChaseVolume = 1.5f;

        [SerializeField] private float _heartChasePitch = 1.5f;

        [Space]
        [SerializeField] private float _heartDamageVolume = 2f;

        [SerializeField] private float _heartDamagePitch = 1.5f;

        [Header("Wind")]
        [SerializeField] private AudioSource _windAudioSource;

        [SerializeField] private float _windChaseTransitionTime = 1f;

        [Space]
        [SerializeField] private float _windChaseVolume = .2f;

        [SerializeField] private float _windCalmVolume = 1f;

        private bool _isChasing;

        private void Start()
        {
            Shark.OnTelegraph += SharkOnTelegraph;
            Shark.OnChase += SharkOnChase;
            Shark.OnPlayerSafe += SharkOnPlayerSafe;

            Player.OnTakeDamage += PlayerOnTakeDamage;
            Player.OnHeal += PlayerOnHeal;
            SetChaseState(false);
        }

        private void PlayerOnHeal()
        {
            _heartBeatAudioSource
                .DOPitch(_isChasing ? _heartChasePitch : _heartCalmPitch, _heartChaseTransitionTime)
                .SetEase(Ease.InOutSine);
            _heartBeatAudioSource
                .DOFade(_isChasing ? _heartChaseVolume : _heartCalmVolume, _heartChaseTransitionTime)
                .SetEase(Ease.InOutSine);
        }

        private void PlayerOnTakeDamage()
        {
            /*_heartBeatAudioSource
                .DOPitch(_heartDamagePitch, _heartChaseTransitionTime)
                .SetEase(Ease.InOutSine);
            _heartBeatAudioSource
                .DOFade(_heartDamageVolume, _heartChaseTransitionTime)
                .SetEase(Ease.InOutSine);*/
        }

        private void SharkOnChase()
        {
            _heartBeatAudioSource
                .DOPitch(_heartDamagePitch, _heartChaseTransitionTime)
                .SetEase(Ease.InOutSine);
            _heartBeatAudioSource
                .DOFade(_heartDamageVolume, _heartChaseTransitionTime)
                .SetEase(Ease.InOutSine);
            _chaseSnapshot.TransitionTo(.1f);
            SetChaseState(true);
        }

        private void SharkOnPlayerSafe()
        {
            _baseSnapshot.TransitionTo(.1f);
            SetChaseState(false);
        }

        private void SharkOnTelegraph(Shark _)
        {
            _telegraphSnapshot.TransitionTo(.1f);
        }

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