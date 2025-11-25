using System;
using System.Collections;
using DG.Tweening;
using Enemies;
using UnityEngine;

namespace PlayerComponents
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        public static event Action<Player> OnSpawn;

        public static event Action OnTakeDamage;

        public static event Action OnHeal;

        public static event Action OnDead;

        public bool IsSafe { get; private set; }

        public bool CanGear => _rigidbody.linearVelocity.With(y: 0f).magnitude < 1f;

        [SerializeField] private float _buoyancy = 1f;
        [SerializeField, Range(0f, 0.1f)] private float _buoyancyRange = .05f;

        [SerializeField] private float _maxSpeed = 10f;
        [SerializeField] private float _maxAngularSpeed = 1f;

        [SerializeField] private float _acceleration = 50f;
        [SerializeField] private float _deceleration = 100f;

        [SerializeField] private float _turnSpeed = 100f;

        [SerializeField] private AudioSource _motorRunningAudio;
        [SerializeField] private float _motorMaxVolume = .75f;
        [SerializeField] private ParticleSystem _foamParticles;

        [Header("Take Damage")]
        [SerializeField] private float _hitForce = 100f;

        [SerializeField] private AudioSource _hitAudio;
        [SerializeField] private AudioSource _deathAudio;

        [SerializeField] private ParticleSystem _deathParticles;

        private Rigidbody _rigidbody;
        private InputReader _input;
        private Coroutine _fadeAsync;

        private Vector2 _direction;

        private bool _goDown;
        private bool _isRunning;
        private bool _injured;
        private bool _isDeath;
        private bool _canTakeDamage;

        private void Awake()
        {
            _input = new InputReader();
            _input.Enable();
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        private void Start()
        {
            _canTakeDamage = true;

            // _rigidbody.maxLinearVelocity = _maxSpeed;
            _rigidbody.maxAngularVelocity = _maxAngularSpeed;

            OnSpawn?.Invoke(this);
        }

        private void FixedUpdate()
        {
            // TODO delete this 2 lines.
            // _rigidbody.maxLinearVelocity = _maxSpeed;
            //_rigidbody.maxAngularVelocity = _maxAngularSpeed;

            var movement = transform.forward * _input.Move.z + transform.right * _input.Move.x;
            var isMoving = movement.magnitude > .05f;
            RunningCheck(isMoving);
            Movement();
            Rotation(movement);
            Buoyancy();
        }

        private void Buoyancy()
        {
            if (_goDown && transform.position.y < 0f)
                _goDown = false;
            else if (!_goDown && transform.position.y > 0f)
                _goDown = true;

            _rigidbody.AddForce(_goDown ? Vector3.down * _buoyancy : Vector3.up * _buoyancy);
        }

        private void Movement()
        {
            if (_input.Move.z > .05f)
            {
                _rigidbody.AddForce(transform.forward * (_acceleration * _input.Move.z), ForceMode.VelocityChange);
            }
            else if (_input.Move.z < -.05f)
            {
                _rigidbody.AddForce(transform.forward * (.5f * _acceleration * _input.Move.z), ForceMode.VelocityChange);
            }

            if (_canTakeDamage)
            {
                var velocity = _rigidbody.linearVelocity;
                velocity.y = 0f;

                if (velocity.magnitude > _maxSpeed)
                {
                    velocity = velocity.normalized * _maxSpeed;
                    velocity.y = _rigidbody.linearVelocity.y;
                    _rigidbody.linearVelocity = velocity;
                }
            }
        }

        private void Rotation(Vector3 targetDirection)
        {
            if (Mathf.Abs(_input.Move.x) < .05f) return;

            Vector3 directionToLook = targetDirection.normalized;

            if (directionToLook == Vector3.zero) return;

            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
            Quaternion rotationDelta = targetRotation * Quaternion.Inverse(_rigidbody.rotation);

            rotationDelta.ToAngleAxis(out float angle, out Vector3 axis);

            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;

            Vector3 torqueVector = axis.normalized * angle * _turnSpeed;

            _rigidbody.AddTorque(torqueVector, ForceMode.Acceleration);
        }

        private void OnTriggerEnter(Collider other)
        {
            var shark = GetComponentFromParents<Shark>(other.transform);
            if (shark != null) TakeDamage(other.transform.position);

            if (other.TryGetComponent(out Beacon _)) IsSafe = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out Beacon _)) return;
            IsSafe = false;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (IsSafe) return;
            var shark = GetComponentFromParents<Shark>(other.transform);
            if (shark == null) return;
            TakeDamage(other.contacts[0].point);
        }

        private void TakeDamage(Vector3 other)
        {
            if (!_canTakeDamage || _isDeath) return;

            var direction = transform.position - other;
            direction.y = 0f;

            _hitAudio.Stop();
            _hitAudio.Play();
            OnTakeDamage?.Invoke();
            DamagePush(direction);

            if (_injured)
            {
                StartCoroutine(DeathAsync());
                return;
            }

            _canTakeDamage = false;
            _injured = true;
            Invoke(nameof(ResetMaxVelocity), .5f);
            Invoke(nameof(Heal), 10f);
        }

        private IEnumerator DeathAsync()
        {
            _isDeath = true;
            _input.Disable();
            yield return new WaitForSeconds(.5f);
            _deathParticles.transform.SetParent(null);
            _deathParticles.transform.position = transform.position;
            _deathParticles.Stop();
            _deathParticles.Play();
            
            _deathAudio.transform.SetParent(null);
            _deathAudio.transform.position = transform.position;
            _deathAudio.Stop();
            _deathAudio.Play();
            OnDead?.Invoke();
            gameObject.SetActive(false);
        }

        private void DamagePush(Vector3 direction)
        {
            // _rigidbody.maxLinearVelocity = _hitForce;
            _rigidbody.AddForce(direction.normalized * _hitForce, ForceMode.VelocityChange);
        }

        private void ResetMaxVelocity()
        {
            // _rigidbody.maxLinearVelocity = _maxSpeed;
            _canTakeDamage = true;
        }

        private void Heal()
        {
            _injured = false;
            OnHeal?.Invoke();
        }

        private void OnDestroy() => _input?.Disable();

        private void RunningCheck(bool isMoving)
        {
            if (_isRunning && !isMoving)
            {
                _foamParticles.Stop();
                _isRunning = false;
                if (_fadeAsync != null) StopCoroutine(_fadeAsync);
                _fadeAsync = StartCoroutine(Fade(fadeIn: false));
            }
            else if (!_isRunning && isMoving)
            {
                _foamParticles.Stop();
                _foamParticles.Play();
                _isRunning = true;
                if (_fadeAsync != null) StopCoroutine(_fadeAsync);
                _fadeAsync = StartCoroutine(Fade(fadeIn: true));
            }
        }

        private IEnumerator Fade(bool fadeIn)
        {
            yield return _motorRunningAudio.DOFade(fadeIn ? _motorMaxVolume : 0f, .5f).WaitForCompletion();
        }

        private T GetComponentFromParents<T>(Transform other) where T : Component
        {
            if (!other.TryGetComponent(out T t))
            {
                if (other.parent == null) return null;
                return GetComponentFromParents<T>(other.parent);
            }
            return t;
        }
    }
}