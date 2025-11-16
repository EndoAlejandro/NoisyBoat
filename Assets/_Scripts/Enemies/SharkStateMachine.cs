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
            var chase = new SharkChaseState(_shark);

            stateMachine.SetState(idle);

            stateMachine.AddTransition(idle, chase, () => _shark.PlayerTransform != null);
            stateMachine.AddTransition(chase, idle, () => _shark.PlayerTransform == null);
        }
    }
}