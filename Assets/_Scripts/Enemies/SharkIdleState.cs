using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    public class SharkIdleState : IState
    {
        public bool CanTransitionToSelf => false;

        private readonly Shark _shark;
        private readonly float _idleTime;

        private Vector3 _target;
        private float _timer;

        private float _growlTimer;

        public SharkIdleState(Shark shark, float idleTime)
        {
            _shark = shark;
            _idleTime = idleTime;
        }

        public void Tick() => Growl();

        public void FixedTick()
        {
            _timer -= Time.fixedDeltaTime;
            if (_timer <= 0f) ResetTarget();
            else
            {
                var direction = _target - _shark.transform.position;
                _shark.Move(direction.normalized);
            }
        }

        public void OnEnter()
        {
            _shark.SetState(isChasing: false);
            _target = _shark.transform.position;
        }

        public void OnExit()
        {
            _timer = 0f;
            _shark.StopGrowl();
        }

        private void ResetTarget()
        {
            _timer = _idleTime;
            var random = Random.insideUnitCircle.normalized * _shark.PatrolRange;
            var pivot = _shark.Player?.transform.position ?? Vector3.zero;
            _target = new Vector3(random.x, 0f, random.y) + pivot;

            if (_target.magnitude > 150f) _target = _target.normalized * 140f;
        }

        private void Growl()
        {
            _growlTimer -= Time.deltaTime;
            if (_growlTimer > 0f) return;

            _growlTimer = 20f;
            _shark.Growl();
        }
    }
}