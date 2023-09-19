using System;
using RootName.Core.BaseMessages;

namespace RootName.Core.BaseStates
{
    internal class StateChangedMessage<TState, TFiniteState> : IMessage
        where TState : IState
        where TFiniteState : IFiniteState
    {
        public TFiniteState PrevState { get; }
        public TFiniteState NextState { get; }

        public StateChangedMessage(TFiniteState prevState, TFiniteState nextState)
        {
            PrevState = prevState;
            NextState = nextState;
        }
    }
}