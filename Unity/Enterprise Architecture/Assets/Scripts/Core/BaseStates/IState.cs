using System;

namespace RootName.Core.BaseStates
{
    internal interface IState
    {
        Type GetFiniteStateType();
    }
}