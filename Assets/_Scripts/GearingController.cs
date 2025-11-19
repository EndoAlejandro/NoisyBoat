using System;
using DG.Tweening;
using Shapes;
using UnityEngine;

public class GearingController : MonoBehaviour
{
    [SerializeField] private float _gearingTime = 5f;
    [SerializeField] private Disc _progressDisc;

    [SerializeField] private Transform _container;
    [SerializeField] private float _displayTime = 1.5f;
    [SerializeField] private float _hideTime = .75f;

    [SerializeField] private Transform _gearing;
    [SerializeField] private float _rotationSpeed;

    [SerializeField] private AudioSource _gearingAudio;

    private float _gearingTimer;
    private float _angle;
    private bool _isGearing;

    private void Start()
    {
        HideGearing();
        _progressDisc.AngRadiansStart = 0f;
    }

    private void Update()
    {
        RotationAnimation();

        if (!_isGearing)
        {
            _progressDisc.AngRadiansEnd = 0f;
            _gearingTimer = 0f;
            return;
        }

        _gearingTimer += Time.deltaTime;
        var angle = (_gearingTimer / _gearingTime) * 360f;
        _progressDisc.AngRadiansEnd = angle * Mathf.Deg2Rad;

        if (_gearingTimer >= _gearingTime)
        {
            HideGearing();
        }
    }

    private void RotationAnimation()
    {
        _angle += _rotationSpeed * Time.deltaTime;
        if (_angle >= 180f) _angle = 0f;

        _gearing.localRotation = Quaternion.Euler(Vector3.forward * _angle);
    }

    [ContextMenu("Display Gearing")]
    private void DisplayGearing()
    {
        _isGearing = true;
        _gearingAudio.Play();
        _gearingAudio.DOFade(1f, _displayTime);
        _container.DOScale(Vector3.one, _displayTime)
            .SetEase(Ease.OutElastic).OnComplete(() => _isGearing = true);
    }

    [ContextMenu("Hide Gearing")]
    private void HideGearing()
    {
        _isGearing = false;
        _gearingAudio.DOFade(0f, _hideTime).OnComplete(_gearingAudio.Stop);
        _container.DOScale(Vector3.one * 0f, _hideTime)
            .SetEase(Ease.InBounce);
    }
}