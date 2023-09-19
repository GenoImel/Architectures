using System;
using RootName.Core.BaseStates;

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