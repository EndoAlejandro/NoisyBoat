using System.Collections.Generic;
using System.Linq;
using PlayerComponents;
using Shapes;
using TMPro;
using UnityEngine;

public class Target : SonarImmediateDrawer
{
    [SerializeField] private TMP_Text _text;

    [Header("Bubble")]
    [SerializeField] private Vector2 _bubbleSize;

    [SerializeField] private Vector2 _bubbleFrequency;
    [SerializeField] private Vector2 _bubbleDuration;

    [Header("Wave")]
    [SerializeField] private Vector2 _waveFrequency;

    [SerializeField] private float _maxWaveSize;
    [SerializeField] private float _waveDuration;

    private List<TargetWaves> _waves;
    private List<TargetWaves> _bubbles;

    private float _waveTimer;
    private float _bubbleTimer;

    private void Awake()
    {
        _waves = new List<TargetWaves>();
        _bubbles = new List<TargetWaves>();
    }

    protected override void Update()
    {
        base.Update();

        DrawWave();
        DrawBubble();
    }

    private void DrawWave()
    {
        _waveTimer -= Time.deltaTime;
        if (_waveTimer > 0f) return;

        _waveTimer = Random.Range(_waveFrequency.x, _waveFrequency.y);
        var wave = GetWave();
        wave.Draw(null);
    }

    private void DrawBubble()
    {
        _bubbleTimer -= Time.deltaTime;
        if (_bubbleTimer > 0f) return;

        _bubbleTimer = Random.Range(_bubbleFrequency.x, _bubbleFrequency.y);
        var bubble = GetBubble();
        bubble.Draw(null);
    }

    private TargetWaves GetWave()
    {
        foreach (TargetWaves wave in _waves
                     .Where(wave => wave is { IsAnimating: false }))
            return wave;

        var newWave = new TargetWaves(_maxWaveSize, _waveDuration);
        _waves.Add(newWave);
        return newWave;
    }

    private TargetWaves GetBubble()
    {
        foreach (TargetWaves wave in _bubbles
                     .Where(wave => wave is { IsAnimating: false }))
            return wave;

        var randomCircle = Random.insideUnitCircle.normalized;
        var randomDistance = Random.Range(0f, _maxWaveSize * .5f);
        var position = new Vector3(randomCircle.x, 0f, randomCircle.y) * randomDistance;
        var randomDuration = Random.Range(_bubbleDuration.x, _bubbleDuration.y);
        var randomSize = Random.Range(_bubbleSize.x, _bubbleSize.y);
        var newWave = new TargetWaves(position, randomSize, randomDuration);
        _bubbles.Add(newWave);
        return newWave;
    }

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            Draw.LineGeometry = LineGeometry.Volumetric3D;
            Draw.ThicknessSpace = ThicknessSpace.Meters;
            Draw.Thickness = .1f;

            Draw.Matrix = transform.localToWorldMatrix;

            var distance = Vector3.Distance(Player.Instance.transform.position, transform.position);
            _text.SetText((distance / _detectionRange).ToString("0.000"));

            foreach (TargetWaves wave in _waves.Where(w => w.IsAnimating).Select(w => w))
            {
                Color color = _baseColor;
                color.a = Mathf.Min(alpha, wave.CurrentAlpha);
                Draw.Ring(Vector3.zero,
                    Quaternion.Euler(Vector3.right * 90f),
                    wave.CurrentSize,
                    color);
            }

            foreach (TargetWaves bubble in _bubbles.Where(w => w.IsAnimating).Select(w => w))
            {
                Color color = _baseColor;
                color.a = Mathf.Min(alpha, bubble.CurrentAlpha);
                Draw.Disc(bubble.Position,
                    Quaternion.Euler(Vector3.right * 90f),
                    bubble.CurrentSize,
                    color);
            }
        }
    }
}