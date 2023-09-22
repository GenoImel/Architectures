using System;
using RootName.Core.States;

namespace RootName.Runtime.States.ApplicationStates
{
    internal abstract class ApplicationFiniteState : IFiniteState
    {
        public Type GetStateType()
        {
            return typeof(ApplicationState);
        }

        internal sealed class LoginState : ApplicationFiniteState
        {
        }

        internal sealed class MainState : ApplicationFiniteState
        {
        }

        internal sealed class LogoutState : ApplicationFiniteState
        {
        }
    }
}