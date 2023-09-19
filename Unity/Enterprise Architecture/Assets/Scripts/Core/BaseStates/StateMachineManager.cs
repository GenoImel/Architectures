using System;
using System.Collections.Generic;

namespace RootName.Core.BaseStates
{
    internal sealed class StateMachineManager
    {
        private readonly IDictionary<Type, IStateMachine> stateMachines =
            new Dictionary<Type, IStateMachine>();

        public void AddStateMachine<TState, TBindTo>(TState stateMachine)
            where TState : TBindTo, IStateMachine
        {
            if (stateMachine == null)
            {
                throw new Exception($"{nameof(stateMachine)} cannot be null");
            }

            var stateType = typeof(TBindTo);
            this.stateMachines[stateType] = stateMachine;
        }

        public TState GetStateMachine<TState>()
        {
            var stateMachineType = typeof(TState);
            if (stateMachines.TryGetValue(stateMachineType, out var stateMachine))
            {
                return (TState)stateMachine;
            }

            throw new Exception($"State Machine {stateMachineType} does not exist");
        }
    }
}