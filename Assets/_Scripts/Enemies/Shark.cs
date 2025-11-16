using System.Collections.Generic;
using Shapes;
using UnityEngine;

namespace Enemies
{
    public class Shark : ImmediateModeShapeDrawer
    {
        public Transform PlayerTransform { get; private set; }

        [SerializeField] private float _playerDetectionRadius = 5f;
        [SerializeField] private float _maxSpeed = 1f;
        [SerializeField] private float _turnSpeed = 1f;
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _deceleration = 20f;

        [SerializeField] private int _nodesAmount = 5;

        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _radius = 1f;

        [SerializeField] private LayerMask _playerLayerMask;

        private SphereCollider _collider;
        private Rigidbody _rigidbody;
        private List<SharkNode> _nodes;

        private Collider[] _results;

        private void Awake()
        {
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
                node.SetSpeed(_speed);
            }
        }

        private void Start()
        {
            _collider.radius = _radius;

            for (int i = 0; i < _nodesAmount; i++)
            {
                var node = new SharkNode(transform.position, _radius, _speed);
                if (i > 0)
                {
                    node.SetHead(_nodes[^1]);
                }
                _nodes.Add(node);
            }
        }

        private void Update()
        {
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
                if (_results[i].TryGetComponent(out PlayerController controller))
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
                node.SetMaxDistance(_radius * (1f - i / (float)_nodes.Count));
                node.Tick();
            }
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

        public override void DrawShapes(Camera cam)
        {
            using (Draw.Command(cam))
            {
                Draw.LineGeometry = LineGeometry.Volumetric3D;
                Draw.ThicknessSpace = ThicknessSpace.Meters;
                Draw.Thickness = 1f;

                for (var i = 0; i < _nodes.Count; i++)
                {
                    SharkNode node = _nodes[i];
                    Draw.Sphere(node.Position, _radius - i / (float)_nodes.Count, Color.black);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _playerDetectionRadius);
        }
    }
}