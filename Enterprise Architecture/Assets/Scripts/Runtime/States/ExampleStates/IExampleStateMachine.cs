// We must use the RootName.Core.States namespace to inherit from IStateMachine.
using RootName.Core.States;

// All StateMachines require an interface to be defined.
//
// The interface of the StateMachines resides in a subfolder of the same name within
// the the "Assets/Runtime/States" folder.
//
// Defining an interface for our StateMachine allows for StateMachines to be generically typed,
// which enables the bootstrapping of StateMachines at runtime. Interfaces also allow us
// to restrict how these StateMachines are used by other classes.
//
// All public methods for the StateMachines must have their method signatures
// defined in the interface itself.
namespace RootName.Runtime.States.ExampleStates
{
    /// <summary>
    /// We should always add a summary to the interface class definition.
    /// This StateMachine sets states based on a panel's color, as an example.
    /// </summary>
    internal interface IExampleStateMachine : IStateMachine
    {
        /// <summary>
        /// We should also always add summaries to interface methods.
        /// As an example: Sets the red state.
        /// </summary>
        public void SetRedState();

        /// <summary>
        /// Sets the blue state.
        /// </summary>
        public void SetBlueState();

        /// <summary>
        /// Sets the green state.
        /// </summary>
        public void SetGreenState();
    }
}