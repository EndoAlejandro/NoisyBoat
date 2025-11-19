using System.Collections.Generic;
using DG.Tweening;
using PlayerComponents;
using Shapes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    public class Shark : SonarImmediateDrawer
    {
        public Transform PlayerTransform { get; private set; }

        [Header("Movement")]
        [SerializeField] private LayerMask _playerLayerMask;

        [SerializeField] private float _playerDetectionRadius = 5f;
        [SerializeField] private float _maxSpeed = 1f;
        [SerializeField] private float _turnSpeed = 1f;
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _deceleration = 20f;

        [Header("Visuals")]
        [SerializeField] private AnimationCurve _sizeCurve;

        [FormerlySerializedAs("_revert")] [SerializeField]
        private bool _reverseCurve;
        [SerializeField] private int _nodesAmount = 10;
        [SerializeField] private float _nodeFollowSpeed = 1f;
        [SerializeField] private float _nodeMaxRadius = 1f;
        
        [SerializeField, ColorUsage(true)] private Color _idleColor;
        [SerializeField, ColorUsage(true)] private Color _chaseColor;

        [field: SerializeField] public float PatrolRange { get; private set; } = 50f;

        private SphereCollider _collider;
        private Rigidbody _rigidbody;
        private List<SharkNode> _nodes;

        private Collider[] _results;
        private Vector3 _initialPosition;

        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
            _rigidbody = GetComponent<Rigidbody>();

            _nodes = new List<SharkNode>();
            _results = new Collider[20];
            
            _initialPosition = Vector3.zero;
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
                var node = new SharkNode(transform.position, GetRadius(i), _nodeFollowSpeed);
                if (i > 0) node.SetHead(_nodes[^1]);
                _nodes.Add(node);
            }

            _collider.radius = GetRadius(0);
        }

        protected override void Update()
        {
            base.Update();
            CheckDistance();
            SeekPlayer();
        }

        private void CheckDistance()
        {
            if (PlayerTransform == null) return;
            if (Vector3.Distance(PlayerTransform.position, transform.position) > _playerDetectionRadius)
                PlayerTransform = null;
        }

        private void SeekPlayer()
        {
            if (PlayerTransform != null) return;
            var size = Physics.OverlapSphereNonAlloc(transform.position, _playerDetectionRadius, _results, _playerLayerMask);

            for (int i = 0; i < size; i++)
            {
                if (_results[i].TryGetComponent(out Player controller))
                {
                    PlayerTransform = controller.transform;
                    break;
                }
            }
        }

        private void FixedUpdate()
        {
            UpdateNodes();
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

        private float GetRadius(int i)
        {
            var normalizedValue = _reverseCurve ? 1 - i / (float)_nodes.Count : i / (float)_nodes.Count;
            var radius = _sizeCurve.Evaluate(normalizedValue) * _nodeMaxRadius;
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
                _baseColor = isChasing ? Color.Lerp(_idleColor, _chaseColor, i) : Color.Lerp(_chaseColor, _idleColor, i);
                i = x;
            }, 1f, 1f);
        }

        public override void DrawShapes(Camera cam)
        {
            using (Draw.Command(cam))
            {
                Draw.BlendMode = ShapesBlendMode.Transparent;
                Draw.LineGeometry = LineGeometry.Volumetric3D;
                Draw.ThicknessSpace = ThicknessSpace.Meters;

                for (var i = 0; i < _nodes.Count; i++)
                {
                    SharkNode node = _nodes[i];
                    var radius = GetRadius(i);
                    _baseColor.a = Mathf.Min(1f, alpha + .5f);
                    Draw.Sphere(node.Position, radius, _baseColor);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _playerDetectionRadius);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_initialPosition, PatrolRange);
        }
    }
}