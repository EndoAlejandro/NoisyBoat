using System;
using System.Collections;
using DG.Tweening;
using Shapes;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Sonar : ImmediateModeShapeDrawer
{
    public static Sonar Instance { get; private set; }

    public static event Action<Transform> OnTargetFound;

    [SerializeField] private float _radius = 50f;
    [SerializeField] private float _timePerMeter = .2f;

    private SphereCollider _collider;
    private Color _color;
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

    private IEnumerator Start()
    {
        _color = Color.black;
        yield return new WaitForSeconds(2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Target target)) return;
        OnTargetFound?.Invoke(target.transform);
    }

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            Draw.LineGeometry = LineGeometry.Volumetric3D;
            Draw.ThicknessSpace = ThicknessSpace.Meters;
            Draw.Thickness = .1f;

            Draw.Matrix = transform.localToWorldMatrix;
            Draw.Ring(Vector3.zero, Quaternion.Euler(Vector3.right * 90f), _currentSize, _color);
        }
    }


    public void Activate(Vector3 position, Action onComplete)
    {
        transform.position = position;
        _collider.enabled = true;

        DOTween.To(() => _collider.radius,
                x =>
                {
                    _currentSize = _collider.radius;
                    _collider.radius = x;
                    var normalized = x / _radius;
                    _color.a = .5f + (1f - normalized);
                }, _radius, _timePerMeter)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                Deactivate();
                onComplete?.Invoke();
            });
    }

    private void Deactivate()
    {
        _currentSize = 0f;
        _collider.radius = 0f;
        _collider.enabled = false;
    }
}