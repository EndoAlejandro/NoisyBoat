using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    public class SharkCagedState : IState
    {
        public bool CanTransitionToSelf => false;

        private readonly Shark _shark;
        private readonly float _changeColorTime;

        private Vector3 _target;
        private float _initTimer;
        private float _restTimer;
        private bool _isChasing;
        private bool _init;

        public SharkCagedState(Shark shark, float changeColorTime)
        {
            _shark = shark;
            _changeColorTime = changeColorTime;
        }

        public void Tick()
        {
            if (_initTimer > 0f)
            {
                _initTimer -= Time.deltaTime;
                _shark.Move(Vector3.zero);
                return;
            }
            
            if(_init) Setup();
        }

        public void FixedTick()
        {
            if (_initTimer > 0f) return;
            _restTimer -= Time.fixedDeltaTime;
            if (_restTimer <= 0f) ResetTarget();

            var direction = _target - _shark.transform.position;
            if (direction.magnitude > 1f) _shark.Move(direction.normalized * .5f);
        }

        public void OnEnter()
        {
            _shark.Caging();
            _shark.Scream();
            _initTimer = 6f;
            _init = true;
        }

        private void Setup()
        {
            _init = false;
            _isChasing = true;
            _shark.Scream();
            _shark.SetState(false);
            _shark.SetMaxRadius(_shark.NodeMaxRadius * .5f);
            _target = _shark.Turret.transform.position.With(y: 0f);
            _shark.Caged();
        }

        private void ResetTarget()
        {
            _restTimer = 5f;
            var random = Random.insideUnitCircle.normalized * .5f;
            var pivot = _shark.Turret?.transform.position.With(y: 0f) ?? Vector3.zero;
            _target = new Vector3(random.x, 0f, random.y) + pivot;
        }

        public void OnExit() { }
    }
}