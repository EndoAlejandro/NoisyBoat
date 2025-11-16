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

        public void OnEnter() => _target = _shark.transform.position;

        private void ResetTarget()
        {
            _timer = _idleTime;
            var random = Random.insideUnitCircle.normalized * 10f;
            _target = new Vector3(random.x, 0f, random.y);
        }

        public void OnExit() => _timer = 0f;
    }
}