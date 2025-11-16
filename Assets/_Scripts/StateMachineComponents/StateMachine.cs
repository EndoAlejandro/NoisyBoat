using System;
using System.Collections.Generic;

namespace StateMachineComponents
{
    public class StateMachine
    {
        public event Action<IState> OnStateChanged;

        private List<StateTransition> _stateTransitions = new List<StateTransition>();
        private List<StateTransition> _anyStateTransitions = new List<StateTransition>();

        private IState _currentState;
        public IState CurrentState => _currentState;

        /// <summary>
        /// Add a state to state transition.
        /// </summary>
        /// <param name="from">The condition is only checked when the current state equals this.</param>
        /// <param name="to">The state to transition.</param>
        /// <param name="condition">If return true, the transition is made.</param>
        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            var stateTransition = new StateTransition(from, to, condition);
            _stateTransitions.Add(stateTransition);
        }

        public void AddManyTransitions(IState[] fromMany, IState to, Func<bool> condition)
        {
            foreach (var from in fromMany) AddTransition(from, to, condition);
        }

        /// <summary>
        /// Add a any to state transition. The current state is not required.
        /// </summary>
        /// <param name="state">The state to transition.</param>
        /// <param name="condition">If return true, the transition is made.</param>
        public void AddAnyTransition(IState state, Func<bool> condition)
        {
            var stateTransition = new StateTransition(null, state, condition);
            _anyStateTransitions.Add(stateTransition);
        }

        public void SetState(IState state)
        {
            if (_currentState is { CanTransitionToSelf: false } && _currentState == state) return;

            _currentState?.OnExit();
            _currentState = state;
            _currentState?.OnEnter();
            OnStateChanged?.Invoke(_currentState);
        }

        public void Tick()
        {
            var transition = CheckForTransition();

            if (transition != null)
                SetState(transition.to);

            _currentState?.Tick();
        }

        public void FixedTick() => _currentState?.FixedTick();

        private StateTransition CheckForTransition()
        {
            foreach (var transition in _anyStateTransitions)
            {
                if (transition.condition())
                    return transition;
            }

            foreach (var transition in _stateTransitions)
            {
                if (transition.from == _currentState && transition.condition())
                    return transition;
            }

            return null;
        }
    }
}