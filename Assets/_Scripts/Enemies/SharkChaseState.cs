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
            if (_shark.Player == null) return;

            if (Vector3.Distance(_shark.transform.position, _shark.Player.transform.position) < 1f)
                return;

            var direction = _shark.Player.transform.position - _shark.transform.position;
            _shark.Move(direction.normalized);
        }

        public void OnEnter()
        {
            _shark.Chase();
            _shark.SetState(isChasing: true);
        }

        public void OnExit()
        {
            _shark.PlayerSafe();
        }
    }
}