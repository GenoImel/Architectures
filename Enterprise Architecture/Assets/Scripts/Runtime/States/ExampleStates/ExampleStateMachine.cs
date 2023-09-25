// - RootNameSpace.Core.Messages is needed in order to create a new StateChangedMessage.
// - RootNameSpace.Core.States is needed in order to inherit from the BaseStateMachine class.
using RootName.Core.Messages;
using RootName.Core.States;

// The namespace of the any scripts for a StateMachine should always match its file/folder location.
namespace RootName.Runtime.States.ExampleStates
{
    // In general when writing the class definition for a StateMachine:
    // - Mark the class as internal to restrict its visibility to the namespace it lives in.
    // - Make the class sealed to prevent further inheritance.
    // - Inherit from BaseStateMachine, which contains MonoBehaviour, so that we can add the
    //   StateMachine to a GameObject as a component. This also enforces the use of specific abstract
    //   methods that are required for a StateChanged Event to work properly.
    // - Inherit from the companion interface, in this case IExampleService.
    internal sealed class ExampleStateMachine : BaseStateMachine, IExampleStateMachine
    {
        // We must use the MonoBehaviour Lifecycle method, Awake(), and use it to set the initial state.
        private void Awake()
        {
            // The use of this method is enforced by the BaseStateMachine class.
            SetInitialState();
        }

        // We must implement all public methods defined in our IExampleService interface.
        // We'll start with RedState(). Here we will set the state of our ExampleStateMachine to the RedState.
        // All the logic for doing so is handled internally and privately by the BaseStateMachine class.
        public void SetRedState()
        {
            // Inside the public method, we will call the SetState method from the BaseStateMachine class.
            // Make sure to hand it an IFiniteState that belongs to ExampleState, or else you'll get an exception. :)
            SetState(new ExampleFiniteState.RedState());
        }

        public void SetBlueState()
        {
            // We do the same for all other publicly available methods for our interface.
            SetState(new ExampleFiniteState.BlueState());
        }

        public void SetGreenState()
        {
            SetState(new ExampleFiniteState.GreenState());
        }

        // The use of this method is enforced by the BaseStateMachine class. We must override it,
        // and define the initial state of the StateMachine.
        //
        // We then call SetInitialState() in Awake() to ensure this gets set at Runtime.
        protected override void SetInitialState()
        {
            currentState = new ExampleFiniteState.RedState();
        }

        // The use of this method is enforced by the BaseStateMachine class. We must override it in order to 
        // return the specific StateChangedMessage that we created for this StateMachine.
        // In this case, we will be returning the ExampleStateChangedMessage that we created earlier.
        protected override IMessage CreateStateChangedMessage(IFiniteState prevState, IFiniteState nextState)
        {
            // We enforce that the constructor parameters for
            // prevState and nextState must be types of ExampleFiniteState.
            return new ExampleStateChangedMessage(
                prevState as ExampleFiniteState,
                nextState as ExampleFiniteState
            );
        }
    }
}