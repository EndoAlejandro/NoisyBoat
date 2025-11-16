using System;

namespace StateMachineComponents
{
    public class StateTransition {
        public readonly IState from;
        public readonly IState to;
        public readonly Func<bool> condition;

        public StateTransition(IState from, IState to, Func<bool> condition) {
            this.from = from;
            this.to = to;
            this.condition = condition;
        }
    }
}