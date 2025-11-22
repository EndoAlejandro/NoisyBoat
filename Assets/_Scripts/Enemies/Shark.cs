using System;
using System.Collections.Generic;
using DG.Tweening;
using PlayerComponents;
using Shapes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class Shark : SonarImmediateDrawer
    {
        public static event Action<Shark> OnTelegraph;

        public static event Action OnChase;

        public static event Action OnPlayerSafe;

        public static event Action<Shark> OnCaging;

        public static event Action OnCaged;

        public bool IsChasing { get; private set; }

        public Turret Turret { get; private set; }

        [Header("Movement")]
        [SerializeField] private LayerMask _playerLayerMask;

        [SerializeField] private float _playerDetectionRadius = 5f;
        [SerializeField] private float _maxSpeed = 1f;
        [SerializeField] private float _turnSpeed = 1f;
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _deceleration = 20f;

        [SerializeField] private Transform _collidersContainer;

        [Header("Visuals")]
        [SerializeField] private AnimationCurve _sizeCurve;

        [SerializeField] private bool _reverseCurve;
        [SerializeField] private int _nodesAmount = 10;
        [SerializeField] private float _nodeFollowSpeed = 1f;

        [field: SerializeField] public float NodeMaxRadius { get; private set; } = 7f;

        [SerializeField, ColorUsage(true)] private Color _idleColor;
        [SerializeField, ColorUsage(true)] private Color _chaseColor;

        [Header("Chase")]
        [field: SerializeField] public float PatrolRange { get; private set; } = 50f;

        [SerializeField] private AudioSource _growlAudio;
        [SerializeField] private AudioSource _chaseAudio;
        [SerializeField] private AudioSource _screamAudio;
        [SerializeField] private AudioClip[] _chaseClips;

        private SphereCollider _collider;
        private Rigidbody _rigidbody;
        private List<SharkNode> _nodes;

        private Collider[] _results;
        private Vector3 _alPosition;
        private Color _currentColor;

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<SphereCollider>();
            _rigidbody = GetComponent<Rigidbody>();

            _nodes = new List<SharkNode>();
            _results = new Collider[20];
        }

        private void OnValidate()
        {
            if (_nodes is null) return;

            foreach (SharkNode node in _nodes)
            {
                node.SetSpeed(_nodeFollowSpeed);
            }
        }

        private void Start()
        {
            for (int i = 0; i < _nodesAmount; i++)
            {
                var nodeCollider = new GameObject($"NodeCollider_{i:00}").AddComponent<SphereCollider>();
                nodeCollider.gameObject.layer = gameObject.layer;
                nodeCollider.transform.SetParent(_collidersContainer, false);
                nodeCollider.isTrigger = true;
                var node = new SharkNode(transform.position, GetRadius(i), _nodeFollowSpeed, nodeCollider);
                if (i > 0) node.SetHead(_nodes[^1]);
                _nodes.Add(node);
            }

            _collider.radius = GetRadius(0);
        }

        public void Setup(Player player) => Player = player;

        protected override void Update()
        {
            base.Update();
            CheckDistance();
            SeekPlayer();
        }

        private void FixedUpdate() => UpdateNodes();

        private void OnCollisionEnter(Collision other)
        {
            if (!other.transform.TryGetComponent(out Player _)) return;
            Scream();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Turret turret) || !turret.CanBeDetected) return;
            Turret = turret;
        }

        public void Growl()
        {
            _growlAudio.volume = 1f;
            _growlAudio.Play();
        }

        public void StopGrowl() => _growlAudio.DOFade(0f, .5f);

        private void CheckDistance()
        {
            if (Player is not { IsSafe: true }) return;

            IsChasing = false;
            OnPlayerSafe?.Invoke();
        }

        private void SeekPlayer()
        {
            if (IsChasing || Turret != null) return;
            var size = Physics.OverlapSphereNonAlloc(transform.position, _playerDetectionRadius, _results, _playerLayerMask);

            for (int i = 0; i < size; i++)
            {
                if (!_results[i].TryGetComponent(out Player player) || player.IsSafe) continue;
                IsChasing = true;
                PlayRandomChaseAudio();
                break;
            }
        }

        private void PlayRandomChaseAudio()
        {
            var i = Random.Range(0, _chaseClips.Length);
            _chaseAudio.PlayOneShot(_chaseClips[i]);
        }


        private void UpdateNodes()
        {
            _nodes[0].SetPosition(transform.position);
            for (int i = 1; i < _nodes.Count; i++)
            {
                SharkNode node = _nodes[i];
                var radius = GetRadius(i);
                node.SetMaxDistance(radius);
                node.Tick();
            }
        }

        public void SetMaxRadius(float nodeMaxRadius)
        {
            DOTween.To(() => NodeMaxRadius, x => NodeMaxRadius = x, nodeMaxRadius, .5f)
                .SetEase(Ease.OutSine);
        }

        private float GetRadius(int i)
        {
            var normalizedValue = _reverseCurve ? 1 - i / (float)_nodes.Count : i / (float)_nodes.Count;
            var radius = _sizeCurve.Evaluate(normalizedValue) * NodeMaxRadius;
            return radius;
        }

        public void Move(Vector3 movement)
        {
            if (_rigidbody == null) return;

            if (movement.magnitude > .05f)
            {
                // Rotation.
                Quaternion targetRotation = Quaternion.LookRotation(movement.normalized);
                Quaternion newRotation = Quaternion.Slerp(_rigidbody.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime);
                _rigidbody.MoveRotation(newRotation);

                _rigidbody.linearVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, transform.forward * _maxSpeed, Time.fixedDeltaTime * _acceleration);
            }
            else
            {
                _rigidbody.linearVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, Vector3.zero, Time.fixedDeltaTime * _deceleration);
            }
        }

        public void SetState(bool isChasing)
        {
            float i = 0f;
            DOTween.To(() => i, x =>
            {
                _currentColor = isChasing ? Color.Lerp(_idleColor, _chaseColor, i) : Color.Lerp(_chaseColor, _idleColor, i);
                i = x;
            }, 1f, 1f);
        }

        public override void DrawShapes(Camera cam)
        {
            using (Draw.Command(cam))
            {
                Draw.BlendMode = ShapesBlendMode.Opaque;
                Draw.LineGeometry = LineGeometry.Volumetric3D;
                Draw.ThicknessSpace = ThicknessSpace.Meters;

                for (var i = _nodes.Count - 1; i >= 0; i--)
                {
                    SharkNode node = _nodes[i];
                    var radius = GetRadius(i);
                    var color = Color.Lerp(_currentColor, _idleColor, i / (float)_nodes.Count);
                    color.a = Mathf.Min(1f, alpha + .5f);
                    Draw.Sphere(node.Position, radius, color);
                }
            }
        }

        public void Telegraph()
        {
            Scream();
            OnTelegraph?.Invoke(this);

            float i = 0f;
            DOTween.To(() => i, x =>
            {
                _currentColor = Color.Lerp(_idleColor, _chaseColor, i);
                i = x;
            }, 1f, 1f);
        }

        public void Scream()
        {
            if (_screamAudio.isPlaying) return;
            _screamAudio.Play();
        }

        public void Chase() => OnChase?.Invoke();

        public void Caging()
        {
            CanBeDetected = false;
            OnCaging?.Invoke(this);
        }

        public void Caged() => OnCaged?.Invoke();

        public void PlayerSafe()
        {
            float i = 0f;
            DOTween.To(() => i, x =>
            {
                _currentColor = Color.Lerp(_chaseColor, _idleColor, i);
                i = x;
            }, 1f, 1f);
            OnPlayerSafe?.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _playerDetectionRadius);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(Player?.transform.position ?? Vector3.zero, PatrolRange);
        }
    }
}