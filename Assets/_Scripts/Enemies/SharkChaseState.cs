using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    public class SharkChaseState : IState
    {
        public bool CanTransitionToSelf => false;

        private readonly Shark _shark;
        private readonly float _idleTime;

        public SharkChaseState(Shark shark) => _shark = shark;

        public void Tick() { }

        public void FixedTick()
        {
            if(_shark.PlayerTransform == null) return;
            
            if (Vector3.Distance(_shark.transform.position, _shark.PlayerTransform.position) < 1f)
                return;

            var direction = _shark.PlayerTransform.position - _shark.transform.position;
            _shark.Move(direction.normalized);
        }

        public void OnEnter()
        {
            _shark.SetState(isChasing: true);
        }

        public void OnExit() { }
    }
}