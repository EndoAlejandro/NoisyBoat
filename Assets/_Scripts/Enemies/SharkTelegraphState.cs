using StateMachineComponents;
using UnityEngine;

namespace Enemies
{
    public class SharkTelegraphState : IState
    {
        public bool Ended => _timer <= 0f;

        private readonly Shark _shark;
        private readonly float _time;

        private float _timer;

        public SharkTelegraphState(Shark shark, float time)
        {
            _shark = shark;
            _time = time;
        }

        public bool CanTransitionToSelf => false;

        public void Tick()
        {
            _timer -= Time.unscaledDeltaTime;
        }

        public void FixedTick() { }

        public void OnEnter()
        {
            _timer = _time;
            _shark.Telegraph();

            Time.timeScale = .25f;
        }

        public void OnExit()
        {
            Time.timeScale = 1f;
        }
    }
}