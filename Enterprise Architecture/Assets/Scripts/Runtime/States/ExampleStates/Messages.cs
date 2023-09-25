// We need to use the RootName.Core.States namespace to inherit from the StateChangedMessage<T> class.
using RootName.Core.States;

// The namespace of the any scripts for a StateMachine should always match its file/folder location.
// 
// For our Message Events specifically, all Message Events must be placed in a script called Messages
// that resides within the namespace where these Message Events are most relevant.
namespace RootName.Runtime.States.ExampleStates
{
    // To create an ExampleStateChangedMessage, we must inherit from the StateChangedMessage<T> class,
    // and provide the type of FiniteState that we are using.
    //
    // This Message, like all other Message Events in our application, should be sealed to end
    // the inheritance chain.
    internal sealed class ExampleStateChangedMessage : StateChangedMessage<ExampleFiniteState>
    {
        // As with all non-empty Message Events, we must provide a public constructor. For our StateChangedMessage<T>
        // events specifically, we will want to provide parameters of the previous and next FiniteStates, and
        // pass them to the base constructor.
        //
        // It is very important to provide the previous and next states in this specific order as method parameters.
        // Mixing them up may cause unintended behaviour.
        public ExampleStateChangedMessage(ExampleFiniteState prevState, ExampleFiniteState nextState)
        {
            PrevState = prevState;
            NextState = nextState;
        }
    }
}