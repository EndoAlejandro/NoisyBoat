using System;
using Shapes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerComponents
{
    public class PlayerSonar : ImmediateModeShapeDrawer
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _sonarCooldown = 2f;
        [SerializeField] private float _noiseCooldown = 2f;

        [Space]
        [SerializeField, Range(1, 360)] private int _linesCount = 12;
        [SerializeField] private float _radius = 2f;
        [SerializeField] private float _maxSize = 2.1f;

        [Header("Visuals")]
        [SerializeField, Range(0f, 10f)] private float _thickness = 4f;
        [SerializeField] private float _increaseDuration = .2f;
        [SerializeField] private float _decreaseSpeed = 1f;
        [SerializeField] private ThicknessSpace _thicknessSpace;
        [SerializeField] private Color _color;

        private SonarDot[] _dots;
        private InputReader _input;
        private float _noiseTimer;

        private bool _isSonarActive;

        private float AngleStep => 360f / _linesCount;

        private void OnValidate() => InitDots();

        private void Awake()
        {
            _input = new InputReader();
            _input.Enable();

            InitDots();
        }

        private void InitDots()
        {
            _dots = new SonarDot[_linesCount];

            for (int i = 0; i < _linesCount; i++)
            {
                float angle = i * AngleStep * Mathf.Deg2Rad;
                var position = GetPoint(angle, _radius);

                _dots[i] = new SonarDot(position, _maxSize, _increaseDuration, _decreaseSpeed);
            }
        }

        private void Start()
        {
            Sonar.OnTargetFound += SonarOnTargetFound;
        }

        private void SonarOnTargetFound(Transform target)
        {
            TriggerSonar(target.position - transform.position);
        }

        private void Update()
        {
            
            foreach (SonarDot sonarDot in _dots) sonarDot.Tick();
            SonarNoise();
            SonarTarget();
        }

        private void SonarTarget()
        {
            if (_isSonarActive || !_input.Sonar) return;
            _isSonarActive = true;
            Sonar.Instance.Activate(transform.position, () => _isSonarActive = false);
        }

        private void SonarNoise()
        {
            _noiseTimer -= Time.deltaTime;

            if (_noiseTimer > 0f || _input.Move.magnitude < .1f) return;

            _noiseTimer = _noiseCooldown;
            for (int i = 0; i < 2; i++)
            {
                var randomDirection = Random.insideUnitCircle * 10f;
                var targetDirection = new Vector3(randomDirection.x, 0f, randomDirection.y);
                TriggerSonar(targetDirection);
            }
        }

        private void TriggerSonar(Vector3 direction)
        {
            foreach (SonarDot sonarDot in _dots)
            {
                var dotProduct = Vector3.Dot(sonarDot.Direction.normalized, direction.normalized);
                if (dotProduct < .9f) continue;
                sonarDot.Increase(dotProduct);
            }
        }

        public override void DrawShapes(Camera cam)
        {
            using (Draw.Command(cam))
            {
                Draw.LineGeometry = LineGeometry.Volumetric3D;
                Draw.ThicknessSpace = _thicknessSpace;
                Draw.Thickness = _thickness;

                Draw.Matrix = transform.localToWorldMatrix;

                foreach (SonarDot sonarDot in _dots)
                {
                    Draw.Line(sonarDot.Position, sonarDot.Position + sonarDot.Direction * sonarDot.CurrentSize, _color);
                }
            }
        }

        private Vector3 GetPoint(float angle, float radius)
        {
            var x = radius * Mathf.Cos(angle);
            var z = radius * Mathf.Sin(angle);

            return new Vector3(x, 0f, z);
        }
    }
}