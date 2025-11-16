namespace StateMachineComponents
{
    public interface IState
    {
        /// <summary>
        /// Allow the State machine to transition to the same state if needed.
        /// </summary>
        bool CanTransitionToSelf { get; }
        
        /// <summary>
        /// Called each Update.
        /// </summary>
        void Tick();

        /// <summary>
        /// Called each FixedUpdate.
        /// </summary>
        void FixedTick();

        /// <summary>
        /// Called when the stateMachine enter this state.
        /// </summary>
        void OnEnter();

        /// <summary>
        /// Called when the stateMachine exit this state.
        /// </summary>
        void OnExit();
    }
}