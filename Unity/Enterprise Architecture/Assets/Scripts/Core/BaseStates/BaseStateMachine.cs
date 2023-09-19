using RootName.Core.BaseMessages;
using UnityEngine;

namespace RootName.Core.BaseStates
{
    internal abstract class BaseStateMachine : MonoBehaviour, IStateMachine
    {
        protected IFiniteState currentState;
        protected IFiniteState prevState;
        
        private void Awake()
        {
            SetInitialState();
        }

        protected void SetState(IFiniteState nextState)
        {
            if (nextState == null)
            {
                Debug.LogError("Next state is null.");
                return;
            }

            if (currentState == nextState)
            {
                Debug.LogWarning($"State Machine is already in \"{nextState}\" state.");
                return;
            }
            
            if (currentState != null && currentState.GetStateType() != nextState.GetStateType())
            {
                Debug.LogError($"Invalid state transition from \"{currentState}\" to \"{nextState}\".");
                return;
            }

            prevState = currentState;
            
            var stateChangedMessage = CreateStateChangedMessage(prevState, nextState);
            ApplicationManager.Publish(stateChangedMessage);
            
            currentState = nextState;
            
            Debug.Log($"State Machine is now in \"{nextState}\" state.");
        }

        protected abstract void SetInitialState();

        protected abstract IMessage CreateStateChangedMessage(IFiniteState prevState, IFiniteState nextState);
    }
}