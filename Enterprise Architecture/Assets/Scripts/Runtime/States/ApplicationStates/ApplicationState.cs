using System;
using RootName.Core.States;

namespace RootName.Runtime.States.ApplicationStates
{
    internal class ApplicationState : IState
    {
        public Type GetFiniteStateType()
        {
            return typeof(ApplicationFiniteState);
        }
    }
}