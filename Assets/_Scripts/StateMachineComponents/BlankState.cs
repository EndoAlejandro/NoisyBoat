namespace StateMachineComponents
{
    public class BlankState : IState
    {
        public override string ToString() => "Idle";

        public bool CanTransitionToSelf => false;

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }
}