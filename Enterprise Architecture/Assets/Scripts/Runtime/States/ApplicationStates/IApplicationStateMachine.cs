using RootName.Core.States;

namespace RootName.Runtime.States.ApplicationStates
{
    internal interface IApplicationStateMachine : IStateMachine
    {
        /// <summary>
        /// Sets the login state.
        /// </summary>
        public void SetLoginState();

        /// <summary>
        /// Sets the main state.
        /// </summary>
        public void SetMainState();

        /// <summary>
        /// Sets the logout state.
        /// </summary>
        public void SetLogoutState();
    }
}