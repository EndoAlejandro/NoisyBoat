using System;
using DG.Tweening;
using Enemies;
using Shapes;
using UnityEngine;

public class Turret : SonarImmediateDrawer
{
    [SerializeField] private Transform _visualObr;
    [SerializeField] private float _leashThickness = 1f;
    [SerializeField] private Line _line;

    [SerializeField] private AudioSource _laserAudio;
    [SerializeField] private Disc _area;

    [SerializeField, ColorUsage(true, false)]
    private Color _color;

    private Transform _target;

    private void Start()
    {
        _line.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!CanBeDetected || !other.TryGetComponent(out Shark shark)) return;
        _target = shark.transform;
        _line.enabled = true;
        _laserAudio.Stop();
        _laserAudio.Play();
        DOTween.To(() => _area.Radius, x => _area.Radius = x, 0f, 5f)
            .SetEase(Ease.OutSine)
            .OnComplete(() => CanBeDetected = false);
    }

    protected override void Update()
    {
        base.Update();
        if (_target == null) return;

        _line.Start = Vector3.up * _visualObr.position.y;
        _line.End = (_target.position - transform.position).With(y: 0f);
    }
}