namespace RootName.Runtime.States.ApplicationStates
{
    internal interface IApplicationStateMachine
    {
        public void SetLoginState();

        public void SetMainState();

        public void SetLogoutState();
    }
}