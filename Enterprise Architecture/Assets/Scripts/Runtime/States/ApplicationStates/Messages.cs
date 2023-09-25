using RootName.Core.States;

namespace RootName.Runtime.States.ApplicationStates
{
    internal sealed class ApplicationStateChangedMessage : StateChangedMessage<ApplicationFiniteState>
    {
        public ApplicationStateChangedMessage(ApplicationFiniteState prevState, ApplicationFiniteState nextState)
        {
            PrevState = prevState;
            NextState = nextState;
        }
    }
}