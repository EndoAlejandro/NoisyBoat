using System;
using DG.Tweening;
using Shapes;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SphereCollider))]
public class Sonar : ImmediateModeShapeDrawer
{
    public static Sonar Instance { get; private set; }

    public static event Action<Transform> OnTargetFound;

    [Header("Sonar")]
    [SerializeField] private float _radius = 50f;

    [SerializeField] private float _timePerMeter = .2f;
    [SerializeField] private float _thickness = 5f;
    [SerializeField] private Color _innerColor;
    [SerializeField] private Color _outerColor;

    [Space]
    [SerializeField] private AudioSource _audio;

    [SerializeField] private AudioClip[] _clips;

    private SphereCollider _collider;
    private float _currentSize;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out SonarImmediateDrawer sonarDrawer)) return;
        sonarDrawer.Detect();
        OnTargetFound?.Invoke(sonarDrawer.transform);
    }

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            if (_currentSize <= 0f) return;

            Draw.LineGeometry = LineGeometry.Volumetric3D;
            Draw.ThicknessSpace = ThicknessSpace.Meters;
            Draw.Thickness = .1f;

            Draw.Matrix = transform.localToWorldMatrix;

            var colors = new DiscColors() {
                innerStart = _innerColor,
                innerEnd = _innerColor,
                outerStart = _outerColor,
                outerEnd = _outerColor,
            };

            Draw.Ring(Vector3.zero, Quaternion.Euler(Vector3.right * 90f), _currentSize, _thickness, colors);
        }
    }

    public void Activate(Vector3 position, Action onComplete)
    {
        transform.position = position;
        _collider.enabled = true;
        _audio.PlayOneShot(GetRandomClip());

        DOTween.To(() => _collider.radius,
                x =>
                {
                    _currentSize = _collider.radius;
                    _collider.radius = x;
                }, _radius, _timePerMeter)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                Deactivate();
                onComplete?.Invoke();
            });
    }

    private AudioClip GetRandomClip()
    {
        int i = Random.Range(0, _clips.Length);
        return _clips[i];
    }

    private void Deactivate()
    {
        _currentSize = 0f;
        _collider.radius = 0f;
        _collider.enabled = false;
    }
}