using PlayerComponents;
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

        public SharkIdleState(Shark shark, float idleTime)
        {
            _shark = shark;
            _idleTime = idleTime;
        }

        public void Tick() { }

        public void FixedTick()
        {
            if (Vector3.Distance(_shark.transform.position, _target) < 1f)
            {
                _timer -= Time.fixedDeltaTime;
                if (_timer <= 0f) ResetTarget();
            }
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

        private void ResetTarget()
        {
            _timer = _idleTime;
            var random = Random.insideUnitCircle.normalized * +_shark.PatrolRange * .5f;
            _target = new Vector3(random.x, 0f, random.y) + Player.Instance.transform.position;
            if (_target.magnitude > _shark.PatrolRange) _target = _target.normalized * _shark.PatrolRange;
        }

        public void OnExit() => _timer = 0f;
    }
}