// - System is needed to returning the typeof State for an individual FiniteState.
//
// - RootName.Core.States is required for inheriting from the IFiniteState interface.
//   This allows us to generically type our different IFiniteStates, enforcing type-safety
//   and allowing us to define IFiniteState definitions without enums.
using System;
using RootName.Core.States;

// The namespace of the any scripts for a StateMachine should always match its file/folder location.
namespace RootName.Runtime.States.ExampleStates
{
    // All State definitions must inherit from the IState interface for type-safety.
    // This prevents us from mixing up different States from other StateMachines.
    internal abstract class ExampleFiniteState : IFiniteState
    {

        // We must provide a method for returning the type of State that this FiniteState belongs to.
        // This concludes the enforcement of type-safety for our StateMachine
        // within our enumless state definition.
        public Type GetStateType()
        {
            return typeof(ExampleState);
        }
        
        // Now we can define some FiniteStates for our StateMachine!
        // To do this we create some empty classes that inherit from our abstract FiniteState class.
        // Sealing these classes prevents them from being inherited from, ending the inheritance chain.
        internal sealed class RedState : ExampleFiniteState
        {
        }
        
        internal sealed class BlueState : ExampleFiniteState
        {
        }
        
        internal sealed class GreenState : ExampleFiniteState
        {
        }
    }
}