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

        private Rigidbody _rigidbody;
        private Vector2 _direction;

        private InputReader _input;

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
            var movement = _input.Move;
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

        private void OnDestroy() => _input?.Disable();
    }
}