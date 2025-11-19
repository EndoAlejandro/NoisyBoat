using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace PlayerComponents
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        [SerializeField] private float _maxSpeed = 10f;

        [SerializeField] private float _acceleration = 50f;
        [SerializeField] private float _deceleration = 100f;

        [SerializeField] private float _turnSpeed = 100f;

        [SerializeField] private AudioSource _motorRunningAudio;
        [SerializeField] private float _motorMaxVolume = .75f;

        private Rigidbody _rigidbody;
        private Vector2 _direction;
        private InputReader _input;

        private bool _isRunning;
        private Coroutine _fadeAsync;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _input = new InputReader();
            _input.Enable();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            var movement = transform.forward * _input.Move.z + transform.right * _input.Move.x;
            var isMoving = Mathf.Abs(_input.Move.z) > .05f;
            RunningCheck(isMoving);

            if (Mathf.Abs(_input.Move.x) > .05f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement.normalized);
                Quaternion newRotation = Quaternion.Slerp(_rigidbody.rotation, targetRotation, (isMoving ? 1f : .5f) * _turnSpeed * Time.fixedDeltaTime);
                _rigidbody.MoveRotation(newRotation);
            }

            if (isMoving)
            {
                _rigidbody.linearVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, transform.forward * _maxSpeed, Time.fixedDeltaTime * _acceleration);
            }
            else
            {
                _rigidbody.linearVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, Vector3.zero, Time.fixedDeltaTime * _deceleration);
            }

            // _motorRunningAudio.volume = _input.Move.magnitude / _maxSpeed;
        }

        private void RunningCheck(bool isMoving)
        {
            if (_isRunning && !isMoving)
            {
                _isRunning = false;
                if (_fadeAsync != null) StopCoroutine(_fadeAsync);
                _fadeAsync = StartCoroutine(Fade(fadeIn: false));
            }
            else if (!_isRunning && isMoving)
            {
                _isRunning = true;
                if (_fadeAsync != null) StopCoroutine(_fadeAsync);
                _fadeAsync = StartCoroutine(Fade(fadeIn: true));
            }
        }

        private IEnumerator Fade(bool fadeIn)
        {
            yield return _motorRunningAudio.DOFade(fadeIn ? _motorMaxVolume : 0f, .5f).WaitForCompletion();
        }

        private void OnDestroy() => _input?.Disable();
    }
}