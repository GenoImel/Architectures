using RootName.Core.BaseStates;

namespace RootName.Runtime.States.ApplicationStates
{
    internal class ApplicationStateChangedMessage : StateChangedMessage<ApplicationState, ApplicationFiniteState>
    {
        public ApplicationStateChangedMessage(ApplicationFiniteState prevState, ApplicationFiniteState nextState)
            : base(prevState, nextState)
        {
        }
    }
}