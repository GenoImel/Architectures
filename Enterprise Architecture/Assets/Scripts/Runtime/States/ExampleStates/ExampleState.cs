// - System is needed to returning the typeof FiniteStates within our State definition.
//
// - RootName.Core.States is required for inheriting from the IState interface.
//   This allows us to generically type our different States, enforcing type-safety
//   and allowing us to define State definitions with FiniteStates without enums.
using System; 
using RootName.Core.States; 

// The namespace of the any scripts for a StateMachine should always match its file/folder location.
namespace RootName.Runtime.States.ExampleStates
{
    // All State definitions must inherit from the IState interface for type-safety.
    // This prevents us from mixing up different States from other StateMachines.
    internal class ExampleState : IState
    {
        // We must provide a method for returning the type of FiniteState that this State uses.
        // This concludes the enforcement of type-safety for our StateMachine
        // within our enumless state definition.
        public Type GetFiniteStateType()
        {
            return typeof(ExampleFiniteState);
        }
    }
}