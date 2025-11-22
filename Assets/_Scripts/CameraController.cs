using System;
using DG.Tweening;
using Enemies;
using PlayerComponents;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [SerializeField] private CinemachineCamera _mainCamera;
    [SerializeField] private CinemachineTargetGroup _groupCamera;
    [SerializeField] private Volume _baseVolume;
    [SerializeField] private CinemachineImpulseSource _impulse;

    [Header("Chase")]
    [SerializeField] private float _chaseTransitionTime = 1f;

    [SerializeField] private Volume _chaseVolume;

    private Tween _currentShakeTween;

    private Vignette _baseVignette;
    private Vignette _chaseVignette;

    private bool _tookDamage;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _baseVolume.profile.TryGet(out _baseVignette);
        _chaseVolume.profile.TryGet(out _chaseVignette);
    }

    private void Start()
    {
        Time.timeScale = 1f;

        Shark.OnTelegraph += SharkOnTelegraph;
        Shark.OnChase += SharkOnChase;
        Shark.OnPlayerSafe += SharkOnPlayerSafe;
        Shark.OnCaging += SharkOnCaging;
        Shark.OnCaged += SharkOnCaged;

        Player.OnSpawn += PlayerOnSpawn;
        Player.OnTakeDamage += PlayerOnTakeDamage;
        Player.OnHeal += PlayerOnHeal;
        Player.OnDead += PlayerOnDead;
    }



    private void OnDestroy()
    {
        Shark.OnTelegraph -= SharkOnTelegraph;
        Shark.OnChase -= SharkOnChase;
        Shark.OnPlayerSafe -= SharkOnPlayerSafe;
        Shark.OnCaging -= SharkOnCaging;
        Shark.OnCaged -= SharkOnCaged;

        Player.OnSpawn -= PlayerOnSpawn;
        Player.OnTakeDamage -= PlayerOnTakeDamage;
        Player.OnHeal -= PlayerOnHeal;
        Player.OnDead -= PlayerOnDead;
    }

    private void DoImpulse() => _impulse.GenerateImpulse();

    private void PlayerOnHeal()
    {
        _tookDamage = false;
        DOTween.To(() => _baseVignette.intensity.value, x => _baseVignette.intensity.value = x, .2f, .5f)
            .SetEase(Ease.InOutSine)
            .SetUpdate(UpdateType.Normal, false);

        DOTween.To(() => _chaseVignette.intensity.value, x => _chaseVignette.intensity.value = x, .2f, .5f)
            .SetEase(Ease.InOutSine)
            .SetUpdate(UpdateType.Normal, false);
    }

    private void PlayerOnTakeDamage()
    {
        _tookDamage = true;
        Time.timeScale = 1f;

        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, .1f, .2f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Normal, true)
            .OnComplete(() =>
            {
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, .5f).SetEase(Ease.InQuart).SetUpdate(UpdateType.Normal, true);
            });


        TweenVignette(_baseVignette, .4f, .5f);
        TweenVignette(_chaseVignette, .4f, .5f);
        DoImpulse();
    }

    private void PlayerOnSpawn(Player player)
    {
        SetFollowTarget(player.transform);
        _groupCamera.Targets[0].Object = player.transform;
    }

    private void SetFollowTarget(Transform target)
    {
        var cameraTarget = _mainCamera.Target;
        cameraTarget.TrackingTarget = target;
        cameraTarget.LookAtTarget = target;
        _mainCamera.Target = cameraTarget;
    }

    private void PlayerOnDead()
    {
        DoImpulse();
        TweenVignette(_baseVignette, .6f, .5f);
        TweenVignette(_chaseVignette, .6f, .5f);
        SetFollowTarget(null);
        Invoke(nameof(Pause), 3f);
    }

    private void Pause()
    {
        Time.timeScale = 0f;
    }

    private void SharkOnChase()
    {
        SetGroupCamera(false, null);
        DOTween.To(() => _baseVolume.weight, x => _baseVolume.weight = x, 0f, _chaseTransitionTime).SetEase(Ease.OutSine);
        DOTween.To(() => _chaseVolume.weight, x => _chaseVolume.weight = x, 1f, _chaseTransitionTime).SetEase(Ease.OutSine);
    }

    private void SharkOnPlayerSafe()
    {
        DOTween.To(() => _baseVolume.weight, x => _baseVolume.weight = x, 1f, _chaseTransitionTime).SetEase(Ease.OutSine);
        DOTween.To(() => _chaseVolume.weight, x => _chaseVolume.weight = x, 0f, _chaseTransitionTime).SetEase(Ease.OutSine);
    }

    private void SharkOnTelegraph(Shark shark)
    {
        SetGroupCamera(true, shark.transform);
    }

    private void SharkOnCaged()
    {
        DoImpulse();
        SetGroupCamera(false, null);
    }

    private void SharkOnCaging(Shark shark)
    {
        DoImpulse();
        SetGroupCamera(true, shark.transform);
    }

    private void TweenVignette(Vignette vignette, float endValue, float duration, Action callback = null)
    {
        DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, endValue, duration)
            .SetUpdate(UpdateType.Normal, isIndependentUpdate: true)
            .SetEase(Ease.OutBounce)
            .OnComplete(() => callback?.Invoke());
    }

    private void SetGroupCamera(bool state, Transform other)
    {
        _groupCamera.Targets[1].Object = other;
        _groupCamera.gameObject.SetActive(state);
    }
}