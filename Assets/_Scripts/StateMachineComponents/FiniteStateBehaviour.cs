using System;
using UnityEngine;

namespace StateMachineComponents
{
    /// <summary>
    /// Base for using the basic configuration to work with a state machine.
    /// </summary>
    public abstract class FiniteStateBehaviour : MonoBehaviour
    {
        [SerializeField] private bool printStateTransition;
        protected StateMachine stateMachine;

        // Each time the state change, this event is called.
        public event Action<IState> OnEntityStateChanged;

        // To read the current state from another scripts.
        public IState CurrentStateType => stateMachine.CurrentState;

        protected virtual void Start()
        {
            // Usually all the references are necessary to inject references in the states.
            References();

            // New State machine is created.
            stateMachine = new StateMachine();
            stateMachine.OnStateChanged += state =>
            {
                if (printStateTransition) Debug.Log(state.ToString());
                OnEntityStateChanged?.Invoke(state);
            };

            // This is where all the states and transition are created.
            StateMachine();
        }

        protected abstract void References();
        protected abstract void StateMachine();
        protected virtual void Update() => stateMachine.Tick();
        protected virtual void FixedUpdate() => stateMachine.FixedTick();
    }
}