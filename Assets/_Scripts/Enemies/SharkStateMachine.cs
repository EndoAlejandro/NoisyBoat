using StateMachineComponents;

namespace Enemies
{
    public class SharkStateMachine : FiniteStateBehaviour
    {
        private Shark _shark;

        protected override void References()
        {
            _shark = GetComponent<Shark>();
        }

        protected override void StateMachine()
        {
            var idle = new SharkIdleState(_shark, 2f);
            var telegraph = new SharkTelegraphState(_shark, 3f);
            var chase = new SharkChaseState(_shark);
            var caged = new SharkCagedState(_shark, .5f);

            stateMachine.SetState(idle);

            stateMachine.AddTransition(idle, telegraph, () => _shark.IsChasing);
            stateMachine.AddTransition(telegraph, chase, () => telegraph.Ended);
            stateMachine.AddTransition(chase, idle, () => !_shark.IsChasing);

            stateMachine.AddAnyTransition(caged, () => _shark.Turret != null);
        }
    }
}