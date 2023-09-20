using RootName.Core.Messages;
using RootName.Core.States;

namespace RootName.Runtime.States.ApplicationStates
{
    internal sealed class ApplicationStateMachine : BaseStateMachine, IApplicationStateMachine
    {
        private void Awake()
        {
            SetInitialState();
        }
        
        public void SetLoginState()
        {
            SetState(new ApplicationFiniteState.LoginState());
        }
        
        public void SetMainState()
        {
            SetState(new ApplicationFiniteState.MainState());
        }
        
        public void SetLogoutState()
        {
            SetState(new ApplicationFiniteState.LogoutState());
        }

        protected override void SetInitialState()
        {
            currentState = new ApplicationFiniteState.LoginState();
        }
        
        protected override IMessage CreateStateChangedMessage(IFiniteState prevState, IFiniteState nextState)
        {
            return new ApplicationStateChangedMessage(
                prevState as ApplicationFiniteState, 
                nextState as ApplicationFiniteState
            );
        }
    }
}