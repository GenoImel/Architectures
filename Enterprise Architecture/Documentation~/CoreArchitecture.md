# Core Architecture

The core architecture for the RootName application follows the philosophy of limiting the number of singletons in the codebase, and instead using a single `ApplicationManager` singleton to act as a central hub to orchestrate cross application communication while maintaining flexibility, scalability, maintainability- all wrapped in a simplistic API that is intuitive for developers of all skills levels to learn and apply. 

The RootName application uses a hybridized core architecture with a foundational philosophy that adheres to the following concepts:

- **Embracing Unity's Native Framework:** Prioritizing a `MonoBehaviour`-centric design that respects Unity's `GameObject`-`Component` pattern.
- **Modular Interfacing:** Implementing specialized `interfaces` like `Services` to foster seamless data and code interchangeability among decoupled application features. Further extension with `EntityServices` interfaces provides a streamlined bridge to interact cohesively with architectures rooted in Unity ECS.
- **Hierarchical State Management:** Using `interfaces` and classes tailored for Hierarchical State Machines (HSMs), underscored by `class`-driven `state` definitions.
- **Efficient Dependency Management:** Incorporating the service locator pattern to ensure swift and streamlined dependency injection across the board.
- **Robust Communication Framework:** Ensuring unhindered cross-application communication through `Message Events` relayed via a dedicated `Message Bus`.

While developers are empowered to quickly get started creating `MonoBehaviour` features and core application components by referencing our `CodingStandards.md` documentation, those who are interested in understanding the hybridized core architecture on a deeper level are encouraged to read through the commented version of core scripts below. 

## Application Manager

The ApplicationManager is the central hub through which various application components and systems communicate. Its core tenets are in its "single-`singleton`" design – as it is intended to be the only `singleton` in the application. This design decision emerges from a philosophy of simplifying access points and reducing the complexity of the application's API.

### Distinct characteristics of the `ApplicationManager`:
- **Singleton By Design:**

    - The uniqueness of the `ApplicationManager` `singleton` ensures that key core components, such as `Services`, can be easily accessed and used across different features. This also reduces potential issues stemming from multiple instance management.

- **Dynamic Instantiation:**

    - The `ApplicationManager` is dynamically instantiated prior to any scene load. This architecture choice ensures that all core dependencies are automatically incorporated into any active scene. As a result, developers can concentrate on the scenes' specific logic without constantly ensuring that foundational dependencies are integrated. While many enterprise-level applications typically use only one or two scenes, having an automatically instantiating `ApplicationManager` opens us up to the use of test scenes for testing new and experimental features outside of our primary application scene.

- **Message Management:**

    - Integrating a `MessageManager`, the `ApplicationManager` offers a decoupled communication methodology. Instead of conventional Unity/C# `actions`, `events`, and `delegates`, it employs `Message Events` to enable flexible communication. The `ApplicationManager` also provides `interfaces` for adding or removing `message listener`s and publishing `Message Events`. These methods streamline and standardize how various application components interact with each other.


- **Service Management and Interaction:**

    - The `ServiceManager` manages `Services` within the application, which facilitates data and code sharing. It also offers methods for retrieving and adding `Services`, `EntityServices`, and `States` and encapsulates the management of these core architectural components, ensuring that their usage are consistent.

- **Extensibility:**

    - Being abstract, the ApplicationManager class has been structured to be extensible, and this extensibility is primarily exposed through the `RootNameApplicationManager`, where various methods are `overriden` in order to tailor the `ApplicationManager`'s behaviour.

Let's start by stepping through the base `ApplicationManager` class to gather a deeper understanding of how we are accomplishing the above concepts:

```csharp
using System;
using RootName.Core.EntityServices;
using RootName.Core.Messages;
using RootName.Core.Services;
using RootName.Core.StateMachines;
using UnityEngine;

// This script is intended to be the only singleton in the application.
// If a need arises for another singleton, we should consider reworking
// the architecture to fit the new use-case.
//
// The ApplicationManager gives access to Services, which enable cross-platform
// code and data sharing. It also provides a Message Bus for creating and
// publishing Message Events, which are a decoupled alternative to Unity/C#
// actions, events, and delegates.
namespace RootName.Core
{
    // The class definition for our ApplicationManager is:
    // - internal to restrict access to Core architecture to the namespace.
    // - abstract, as this class is inherited by the RootNameApplicationManager script.
    // - Inherits from MonoBehaviour, so the core architecture works within
    //   Unity's MonoBehaviour framework.
   internal abstract class ApplicationManager : MonoBehaviour
    {
        // We need to reference the ApplicationManager prefab that exists in "Assets/Resources".
        private const string ApplicationManagerPrefabPath = "ApplicationManager";
        private static ApplicationManager instance; // The ApplicationManager is our only singleton.
        
        private readonly MessageManager messageManager = new (); // Used for managing Message Events
        private readonly StateMachineManager stateMachineManager = new(); // Used for managing StateMachines.
        private readonly ServiceManager serviceManager = new (); // Used for managing Services.
        private readonly EntityServiceManager entityServiceManager = new (); // Used for managing EntityServices.

        // At runtime, prior to any scene being loaded, we instantiate the RootNameApplicationManager prefab.
        // This is convenient because it means that we can worry less about scenes containing the
        // correct scripts in order to run. All core dependencies will simply be loaded into any 
        // active scene when hitting the play button in the editor, or running the application in a standalone player.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // If an instance of ApplicationManager already exists, return.
            // No need to initialize a new one.
            if (instance)
            {
                return;
            }

            // Load the ApplicationManager prefab from resources and instantiate it into the scene.
            var applicationManagerPrefab = Resources.Load<ApplicationManager>(ApplicationManagerPrefabPath);
            var applicationManager = Instantiate(applicationManagerPrefab);

            // Give the prefab a name. This name will match the name of the script component.
            // For this codebase, that will be RootNameApplicationManager.
            applicationManager.name = applicationManager.GetApplicationName();
            
            // Our ApplicationManager should be persistent between scenes.
            DontDestroyOnLoad(applicationManager);

            // Assign the instance to the ApplicationManager we just instantiated.
            instance = applicationManager;

            // Call the protected method OnInitialized(). This is empty in this abstract script,
            // but has some logic in RootNameApplicationManager, which inherits from ApplicationManager.
            applicationManager.OnInitialized();
        }

        /// <summary>
        /// Adds a new Message Listener.
        /// </summary>
        public static void AddListener<TMessage>(Action<TMessage> listener) where TMessage : IMessage
        {
            instance.messageManager.AddListener(listener);
        }

        /// <summary>
        /// Removes a Message Listener.
        /// </summary>
        public static void RemoveListener<TMessage>(Action<TMessage> listener) where TMessage : IMessage
        {
            instance.messageManager.RemoveListener(listener);
        }

        /// <summary>
        /// Publishes a Message Event across the Message Bus.
        /// </summary>
        public static void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            instance.messageManager.Publish(message);
        }
        
        /// <returns>
        /// Returns an existing State Machine in the application.
        /// </returns>
        public static TStateMachine GetStateMachine<TStateMachine>()
        {
            return instance.stateMachineManager.GetStateMachine<TStateMachine>();
        }
        
        /// <summary>
        /// Adds a State Machine to the application.
        /// </summary>
        protected void AddStateMachine<TStateMachine, TBindTo>(TStateMachine stateMachine) 
            where TStateMachine : IStateMachine, TBindTo
        {
            stateMachineManager.AddStateMachine<TStateMachine, TBindTo>(stateMachine);
        }

        /// <returns>
        /// Returns an existing Service in the application.
        /// </returns>
        public static TService GetService<TService>()
        {
            return instance.serviceManager.GetService<TService>();
        }

        /// <summary>
        /// Adds a Service to the application.
        /// </summary>
        protected void AddService<TService, TBindTo>(TService service) 
            where TService : IService, TBindTo
        {
            serviceManager.AddService<TService, TBindTo>(service);
        }

        /// <returns>
        ///Returns an existing Entity Service in the application.
        /// </returns>
        public static TEntityService GetEntityService<TEntityService>()
        {
            return instance.entityServiceManager.GetEntityService<TEntityService>();
        }

        /// <summary>
        /// Adds an Entity Service to the application.
        /// </summary>
        protected void AddEntityService<TEntityService, TBindTo>(TEntityService entityService)
            where TEntityService : IEntityService, TBindTo
        {
            entityServiceManager.AddEntityService<TEntityService, TBindTo>(entityService);
        }
        
        /// <returns>
        /// Name of the application or application.
        /// </returns>
        protected abstract string GetApplicationName();

        /// <summary>
        /// Called when the application manager is initialized.
        /// </summary>
        protected abstract void OnInitialized();

        /// <summary>
        /// Called when bootstrapping application state machines.
        /// </summary>
        protected abstract void InitializeApplicationStateMachines();
        
        /// <summary>
        /// Called when bootstrapping application services.
        /// </summary>
        protected abstract void InitializeApplicationServices();
        
        /// <summary>
        /// Called when bootstrapping application entity services.
        /// </summary>
        protected abstract void InitializeApplicationEntityServices();

        /// <summary>
        /// Called after bootstrapping complete.
        /// Sets all parent transforms active.
        /// </summary>
        protected abstract void SetParentsActive();
    }
}
```

The `ApplicationManager` class serves as a foundational element in the RootName application's architecture. Through its design, it centralizes management of key components such as `Message Events`, `StateMachines`, `Services`, and `EntityServices`. With a systematic initialization process, it ensures that essential dependencies are loaded in every scene. By offering methods for listener management, state machine, and service handling, as well as abstracted methods for initialization and setting up services, it promotes a modular and organized approach. In essence, this class provides a simplified API for managing and orchestrating core functionalities within the application.

## Message Events

In the RootName application's architecture, `Message Events` play a pivotal role in facilitating communication between different decoupled features, `StatMachines`, `Services`, and `EntityServices`. This section delves into the structural underpinnings of these `Message Events`, covering everything from the foundational `IMessage` interface, the implementation of a `MessageListener`, and to the management of `Message Events` by the `MessageManager`. Each of these elements collectively ensures decoupled and efficient messaging across the application.

### IMessage

At the core of the messaging system within the RootName application lies the `IMessage` `interface`. Serving as a foundational touch point for all `Message Events`, `IMessage` offers a level of abstraction that ensures consistent typing across the entire codebase. Generic typing makes sure that any `Message Event`, regardless of its unique characteristics, can be incorporated seamlessly into the `MessageListener`. 

```csharp
namespace RootName.Core.Messages
{
    // The IMessage interface is used to generically type all
    // Message Events so that regardless of the overlying type
    // of the Message Event, they can be added to the Message Listener
    // since they all inherit from the generic type of IMessage.
    //
    // When creating a new Message Event, we will inherit from
    // this IMessage interface.
    internal interface IMessage
    {
    }
}
```

### Message Listener

The `MessageListener` serves a crucial role within our messaging system. Acting as an intermediary, it tracks and manages `Message Events` using an `IList`. This class allows scripts to register or publish specific `Message Events` via the `Message Bus`. In the details below, we'll break down its structure and the methods.

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

// The MessageListener script provides an IList of available Message Events that 
// other scripts can listen for by using ApplicationManager.AddListener().
// The MessageListener also allows scripts to publish Message Events over the
// Message Bus by using ApplicationManager.Publish().
namespace RootName.Core.Messages
{
    internal sealed class MessageListener<TMessage> : MessageListener where TMessage : IMessage
    {
        // This IList of MessageListeners is used to track all TMessage : IMessage.
        // listeners are added to the list during AddListener() and
        // removed from the list during RemoveListener().
        //
        // Multiple scripts may be listening for the same type of IMessage.
        // So for each type of IMessage, a listener exists which is
        // stored in an IDictionary<Type, MessageListener> in our MessageManager.
        private readonly IList<Action<TMessage>> listeners = new List<Action<TMessage>>();

        // Used for housekeeping purposes. We'll need to iterate through the list at some points.
        public int ListenerCount => listeners.Count;

        // Add a listener for an TMessage : IMessage to our IList listeners.
        public void AddListener(Action<TMessage> listener)
        {
            listeners.Add(listener);
        }

        // Remove a listener for an IMessage : IMessage to our IList listeners.
        public void RemoveListener(Action<TMessage> listener)
        {
            listeners.Remove(listener);
        }

        // Publish a TMessage : IMessage over the Message Bus.
        public void Publish(TMessage message)
        {
            // Iterate over the list to make sure the Message Event exists.
            for (var index = listeners.Count - 1; index >= 0; index--)
            {
                var listener = listeners[index];
                try
                {
                    // If the Message Event is found in our IList listeners,
                    // invoke the Message Event.
                    listener.Invoke(message);
                }
                catch(Exception exception)
                {
                    // If the Message Event is not found in our IList listeners
                    // log an exception.
                    Debug.LogException(exception);
                }
            }
        }
    }
    
    // Abstract class to assist with generic typing via TMessage : IMessage
    internal abstract class MessageListener
    {
    }
}
```

### Message Manager

The `MessageManager` is the orchestrator of our `Message Bus`, centralizing the management of all `listeners` for each `IMessage` type. By using a structured `IDictionary`, it keeps track of `MessageListeners` and efficiently directs `Message Event` communication. Let's breakdown the `MessageListener` script to grasp how this manager optimizes the processes of adding, removing, and publishing messages in our architecture.

```csharp
using System;
using System.Collections.Generic;

// The MessageManager script keeps an IDictionary<Type, MessageListener>
// in order to keep track of all MessageListeners of type IMessage
// that have been added, and remove MessageListeners. It also handles
// further logic for publishing Message Events via MessageListener.
namespace RootName.Core.Messages
{
    // This class is internal to the Core.Messages namespace, and sealed to prevent 
    // extension through inheritance.
    internal sealed class MessageManager
    {
        // Create an IDictionary to pair our MessageListener of type IMessage
        // with their overlying Type as a key.
        private readonly IDictionary<Type, MessageListener> listeners =
            new Dictionary<Type, MessageListener>();

        // Adding a new MessageListener to the Message Bus.
        public void AddListener<TMessage>(Action<TMessage> listener) where TMessage : IMessage
        {
            // First we get the overlying type of the TMessage : IMessage.
            var listenerType = typeof(TMessage);
            
            // We then determine whether this MessageListener already exists in our
            // IDictionary of MessageListeners.
            if(listeners.TryGetValue(listenerType, out var existingMessageListener))
            {
                // If the Message Event we are adding already has an existing
                // Message Listener, we access the MessageListener associated 
                // with the listener's type, and then add the new Message Event
                // to that particular MessageListener.
                var typedMessageListener = (MessageListener<TMessage>)existingMessageListener;
                typedMessageListener.AddListener(listener);
            }
            else
            {
                // If the Message Event we are adding does not already have
                // an existing Message Listener, we create a new MessageListener
                // and then add the listener to the new MessageListener.
                var newMessageListener = new MessageListener<TMessage>();
                newMessageListener.AddListener(listener);

                // We also make sure to add this new <Type, MessageListener> pair to our
                // IDictionary listeners.
                listeners[listenerType] = newMessageListener;
            }
        }

        // Removing a MessageListener from the Message Bus.
        public void RemoveListener<TMessage>(Action<TMessage> listener) where TMessage : IMessage
        {
            // First we get the overlying type of the TMessage : IMessage.
            var listenerType = typeof(TMessage);
            
            // We then determine whether this MessageListener already exists in our
            // IDictionary of MessageListeners.
            if (listeners.TryGetValue(listenerType, out var existingMessageListener) == false)
            {
                // If it doesn't exist, we simply return. Nothing can be removed.
                return;
            }

            // If the MessageListener does exist, we access the MessageListener associated 
            // with the listener's type, and then remove the Message Event
            // from that particular MessageListener.
            var typedMessageListener = (MessageListener<TMessage>)existingMessageListener;
            typedMessageListener.RemoveListener(listener);

            // If all Message Events have been removed from the MessageListener
            // for this particular type, then remove the listener from our 
            // IDictionary of listeners.
            if (typedMessageListener.ListenerCount <= 0)
            {
                listeners.Remove(listenerType);
            }
        }

        // Publishing a Message Event over the Message Bus.
        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            // First we get the overlying type of the TMessage : IMessage.
            var listenerType = typeof(TMessage);
            
            // We then determine whether this MessageListener exists in our
            // IDictionary of MessageListeners.
            if (listeners.TryGetValue(listenerType, out var existingMessageListener) == false)
            {
                // If it doesn't exist, we simply return. Nothing can be published.
                return;
            }

            // If the MessageListener does exist, we access the MessageListener associated 
            // with the listener's type, and then publish the TMessage message
            // through the MessageListener associated with the Message Event.
            var typedMessageListener = (MessageListener<TMessage>)existingMessageListener;
            typedMessageListener.Publish(message);
        }
    }
}
```

## Services

`Services` streamline the core architecture by replacing the need for multiple `singletons`, ensuring efficient data and code sharing across the application. Instead of managing various prefab `singleton` dependencies, the `ApplicationManager` centralizes these `Services`, simplifying our application's overall API. `ApplicationManager.GetService<TService>()` makes it easy to integrate `Services` via direct dependency injection. This section delves into the technical details and implementation of `Services` within our architectural framework.

### IService

The `IService` `interface` serves a crucial role by enabling generic typing for all `Services` within the core architecture. This approach allows Services to be be intuitively accessed through the service locator pattern in the `ApplicationManager`. This section delves into the specifics of the `IService` `interface` and its role in facilitating efficient and type-safe `Service` retrieval.

```csharp
namespace RootName.Core.Services
{
    // The IService interface is used to generically type all
    // Services so that regardless of the overlying type
    // of the Service, they can be bootstrapped into the
    // ApplicationManager at runtime.
    
    /// <summary>
    /// For generic typing of specific Services.
    /// </summary>
    internal interface IService
    {
    }
}
```

### Service Manager

The `ServiceManager` is an integral component responsible for managing and providing access to the various `Services` within the application. Through its robust methods, it facilitates the bootstrapping of `Services` and ensures that dependencies are injected wherever they are needed. By using the service locator pattern, `ServiceManager` works within the `ApplicationManager` to provide scripts with the requested `Services` through `ApplicationManager.GetService<TService>()`. This section elaborates on the mechanics of the `ServiceManager` and how it optimizes `Service` management and retrieval.

```csharp
using System;
using System.Collections.Generic;

// The Service Manager allows us to bootstrap Services generically typed
// via IService by using the AddService method. It also allows us to
// inject dependencies on Services in other scripts by using GetService.
namespace RootName.Core.Services
{
    // This class is internal to the Core.Services namespace, and sealed to prevent 
    // extension through inheritance.
    internal sealed class ServiceManager
    {
        // Create an IDictionary to pair each IService with its type.
        private readonly IDictionary<Type, IService> services =
            new Dictionary<Type, IService>();

        // Bootstrap a Service into the application at runtime. This is called in
        // OnInitialized() in the RootNameApplicationManager.
        //
        // When bootstrapping a Service, we grab the Service's companion
        // MonoBehaviour class as a component from a GameObject.
        // The GameObject that the Service MonoBehaviour class resides on
        // should be parented to the Services child GameObject of our
        // ApplicationManager prefab (found in resources).
        //
        // We then bind the TService : IService to its companion
        // MonoBehaviour class.
        public void AddService<TService, TBindTo>(TService service)
            where TService : TBindTo, IService
        {
            // The GameObject that the Service MonoBehavior component
            // lives on cannot be null. Throw an exception if it is.
            //
            // If this happens, you probably forgot to add your child Service
            // GameObject to its serialized field in the ApplicationManager prefab.
            if (service == null)
            {
                throw new NullReferenceException($"{nameof(service)} cannot be null");
            }

            // If the GameObject for the Service isn't null:
            // - Grab the overlying type from the Service's interface.
            // - Add <Type, IService> pair to the IDictionary of Services.
            var serviceType = typeof(TBindTo);
            services[serviceType] = service;
        }

        // When getting a Service from the ApplicationManager singleton, all
        // that is required is specifying the type of Service you are
        // trying to get.
        public TService GetService<TService>()
        {
            // First we get the overlying type of the requested Service.
            var serviceType = typeof(TService);
            
            // We then check the IDictionary of Services for this type of Service.
            if (services.TryGetValue(serviceType, out var service))
            {
                // If this type of Service exists in the IDictionary, we return
                // it as a TService : IService.
                return (TService)service;
            }

            // If the type of Service was not found in our IDictionary of Services,
            // we throw an Exception.
            //
            // Typically when we run into this issue, it is because we forgot to
            // drag and drop the child GameObject associated with the Service into
            // the serialized field on the ApplicationManager prefab.
            throw new Exception($"Service {serviceType} does not exist");
        }
    }
}
```

## EntityServices

Within the hybrid core framework, `EntityServices` stand as a structured approach to seamlessly integrate the intricacies of Entities, Components, and Systems in an ECS-driven architecture. This core architectural component prioritizes the `MonoBehaviour` orientation of our application, ensuring a cleaner development environment. At the same time, it grants the flexibility to harness the efficiency of ECS. Much like standard `Services`, `EntityServices` facilitate code and data sharing across the application with clean dependency injection via the service locator pattern. 

Currently, the methodology for establishing an `EntityService` mirrors that of a conventional `Service`. Moving forward, we will introduce an `internal abstract class` derived from `MonoBehaviour`. This class will impose the implementation of specific methods tailored for interactions within the ECS segments of the architecture. The exact design of `EntityServices` remain a work in progress, and will not be fully developed until an opportunity to create our first `EntityService` arises. Once we arrive at that bridge, this segment will be updated with a deep dive on the architectural anatomy of `EntityServices`.

## State Machines

In the heart of the RootName core architecture we have `StateMachines`. These constructs are used for orchestrating the distinct phases, behaviors, or operational modes of various application components. Their intricate design allows for a robust and type-safe approach to manage `state` transitions without leaning on less robust methods such as `enums`. Let's take an in-depth look into how `StateMachines` are implemented in our core architecture:

### IState

The `IState` `interface` offers generic typing of specific `state` types, tailored for each individual `StateMachine`. By returning its type, it ensures type-safety and clear differentiation between `states` belonging to specific `StateMachines`.

```csharp
using System;

namespace RootName.Core.StateMachines
{
    // The IState interface is used to generically type all
    // States, and is the beginning of our class-based State
    // definitions for individual StateMachines.

    /// <summary>
    /// For generic typing of specific state types on a per-State Machine basis.
    /// Returns the type of IState for type safety.
    /// </summary>
    internal interface IState
    {
        Type GetFiniteStateType();
    }
}
```

### IFiniteState

In tandem with `IState`, the `IFiniteState` `interface` takes a more granular approach. It is designed to specify `finite states` within an `IState`. Through this `interface`, we eliminate the need for `enums` and instead rely on class-based `state` definitions, again ensuring type-safe operation.

```csharp
using System;

namespace RootName.Core.StateMachines
{
    // IFiniteStates allow us to define specific finite states within an IState
    // using class-based definitions for IFiniteStates rather than enums.
    
    /// <summary>
    /// Allows us to define specific finite states within an <see cref="IState"/>.
    /// This enables us to define state machines without enums.
    /// Interface returns the type of <see cref="IFiniteState"/> for type safety.
    /// </summary>
    internal interface IFiniteState
    {
        Type GetStateType();
    }
}
```

### StateChangedMessage

`Message Events` are essential for communicating `state` changes within our individual `StateMachines`. The `StateChangedMessage` serves as a specialized `Message Event` for any `state` changes, and establishes a specific pattern for `state` transitions that is uniform across the application, ensuring clarity and reducing chances unexpected application behaviour.

```csharp
using RootName.Core.Messages;

namespace RootName.Core.StateMachines
{
    // When defining a StateChangedMessage Event, we must inherit
    // from StateChangedMessage class specifically.
    
    /// <summary>
    /// Base Message Event for State Changes.
    /// Enforces specific state change pattern across the application.
    /// </summary>
    internal class StateChangedMessage<TFiniteState> : IMessage where TFiniteState : IFiniteState
    {
        // It is very important that we specify the previous and next states
        // for our StateChangedMessage. Using other states violates our
        // state change pattern and may lead to unexpected application behaviour.
        public TFiniteState PrevState { get; protected set; }
        public TFiniteState NextState { get; protected set; }
        
        public StateChangedMessage() { }
    }
}
```

### BaseStateMachine

The `BaseStateMachine` class functions as the backbone for the `MonoBehaviour` companion script for our `StateMachines`. Through it, we manage `state` transitions, establish initial `states`, and propagate messages about these transitions. Its design enforces a specific `state` change pattern, which guarantees that the application responds uniformly to `state` changes.

```csharp
using System;
using RootName.Core.Messages; // We use the Core.Messages namespace to create a StateChangedMessage.
using UnityEngine;

namespace RootName.Core.StateMachines
{
    // All StateMachines will inherit from our BaseStateMachine abstract class.
    internal abstract class BaseStateMachine : MonoBehaviour, IStateMachine
    {
        // All StateMachines must have fields for the current state and a previous state.
        protected IFiniteState currentState;
        protected IFiniteState prevState;
        
        // The SetState(IFiniteState nextState) method is used to enforce adherence 
        // of a state change pattern within a particular StateMachine. 

        /// <summary>
        /// Sets the next state of the State Machine and publishes a State Changed Message.
        /// Used to enforce specific state change pattern across the application.
        /// </summary>
        /// <param name="nextState"></param>
        protected void SetState(IFiniteState nextState)
        {
            if (nextState == null)
            {
                // If the next state is null, throw an Exception.
                // If you get this, something has gone wrong.
                throw new Exception("Next state is null.");
            }

            if (currentState == nextState)
            {
                // If we are already in the next state, we should log a warning.
                // This may indicate that something in the application is not behaving as expected.
                Debug.LogWarning($"State Machine is already in \"{nextState}\" state.");
                return;
            }
            
            if (currentState != null && currentState.GetStateType() != nextState.GetStateType())
            {
                // If we have somehow received a state that is not the same type as the IState 
                // for a particular StateMachine, we throw an Exception.
                throw new Exception($"Invalid state transition from \"{currentState}\" to \"{nextState}\".");
            }

            // Set the previous state to the current state.
            prevState = currentState;
            
            // Create a StateChangedMessage Event and use the previous state and next state as parameters,
            // then publish the StateChangedMessage Event.
            var stateChangedMessage = CreateStateChangedMessage(prevState, nextState);
            ApplicationManager.Publish(stateChangedMessage);
            
            // Now that the StateChangedMessage Event has been published, we can
            // set the current state to the next state.
            currentState = nextState;
            
            // This is only here for console based debugging purposes.
            Debug.Log($"State Machine is now in \"{nextState}\" state.");
        }
        
        // Additionally, all StateMachines must implement a method for setting the initial state.
        // This should be called during Awake().

        /// <summary>
        /// Sets the initial state of the State Machine.
        /// Must be called during Awake().
        /// </summary>
        protected abstract void SetInitialState();
        
        // Lastly, all StateMachines must implement a method for creating a StateChangedMessage Event.

        /// <summary>
        /// Creates a State Changed Message while enforcing adherence of a state change pattern
        /// that communicates specifically <paramref name="prevState"/> and <paramref name="nextState"/>.
        /// </summary>
        protected abstract IMessage CreateStateChangedMessage(IFiniteState prevState, IFiniteState nextState);
    }
}
```

### IStateMachine

Much like its sibling `interfaces`, the `IStateMachine` `interface` ensures generic typing for specific `StateMachines`. The use of generic typing accomplishes much the same goal as it does for our `Services` and `EntityServices` in that it allows us to leverage the service locator pattern to achieve clean dependency injection of a specific `StateMachine` without the need for tight coupling.

```csharp
namespace RootName.Core.StateMachines
{
    // The IStateMachine interface is used to generically type all
    // StateMachines so that regardless of the overlying type
    // of the StateMachine, they can be bootstrapped into the
    // ApplicationManager at runtime.
    
    /// <summary>
    /// For generic typing of specific state machines.
    /// Allows us to use the Service locator pattern in
    /// the <see cref="ApplicationManager"/> to get a specific state machine.
    /// </summary>
    internal interface IStateMachine
    {
    }
}
```

### State Machine Manager

The `StateMachineManager` plays a pivotal role in the RootName core architecture by managing `StateMachine` instances, and providing access to them through the service locator pattern for clean dependency injection. It provides methods to add and retrieve `StateMachines`, and much like the `MessageManager` and `ServiceManager`, it uses an `IDictionary` to map `StateMachine` types to their instances, simplifying dependency injection and making them freely available throughout our `ApplicationManager`.

```csharp
using System;
using System.Collections.Generic;

// The StateMachineManager allows us to bootstrap StateMachines generically typed
// via IStateMachine by using the AddStateMachine() method. It also allows us to
// inject dependencies on StateMachines in other scripts by using GetStateMachine().
namespace RootName.Core.StateMachines
{
    // This class is internal to the Core.Services namespace, and sealed to prevent 
    // extension through inheritance.
    internal sealed class StateMachineManager
    {
        // Create an IDictionary to pair each IStateMachine with its type.
        private readonly IDictionary<Type, IStateMachine> stateMachines =
            new Dictionary<Type, IStateMachine>();

        // Bootstrap a StateMachine into the application at runtime. This is called in
        // OnInitialized() in the RootNameApplicationManager.
        //
        // When bootstrapping a StateMachine, we grab the StateMachine's companion
        // MonoBehaviour class as a component from a GameObject.
        // The GameObject that the StateMachine MonoBehaviour class resides on
        // should be parented to the StateMachines child GameObject of our
        // ApplicationManager prefab (found in resources).
        //
        // We then bind the TService : IStateMachine to its companion
        // MonoBehaviour class.
        public void AddStateMachine<TState, TBindTo>(TState stateMachine)
            where TState : TBindTo, IStateMachine
        {
            // The name of the GameObject that the StateMachine MonoBehavior component
            // lives on cannot be null. Throw an exception if it is.
            //
            // If this happens, you probably forgot to add your child StateMachine
            // GameObject to its serialized field in the ApplicationManager prefab.
            if (stateMachine == null)
            {
                throw new NullReferenceException($"{nameof(stateMachine)} cannot be null");
            }

            // If the GameObject for the StateMachine isn't null:
            // - Grab the overlying type from the StateMachine's interface.
            // - Add <Type, IStateMachine> pair to the IDictionary of StateMachines.
            var stateType = typeof(TBindTo);
            stateMachines[stateType] = stateMachine;
        }

        // When getting a StateMachine from the ApplicationManager singleton, all
        // that is required is specifying the type of StateMachine you are
        // trying to get.
        public TState GetStateMachine<TState>()
        {
            // First we get the overlying type of the requested StateMachine.
            var stateMachineType = typeof(TState);
            
            // We then check the IDictionary of Services for this type of StateMachine.
            if (stateMachines.TryGetValue(stateMachineType, out var stateMachine))
            {
                // If this type of StateMachine exists in the IDictionary, we return
                // it as a TStateMachine : IStateMachine.
                return (TState)stateMachine;
            }

            // If the type of StateMachine was not found in our IDictionary of StateMachines,
            // we throw an Exception.
            //
            // Typically when we run into this issue, it is because we forgot to
            // drag and drop the child GameObject associated with the StateMachine into
            // the serialized field on the ApplicationManager prefab.
            throw new Exception($"State Machine {stateMachineType} does not exist");
        }
    }
}
```

## RootName Application Manager

The `RootNameApplicationManager` is our final stop in our architectural deep-dive. The `RootNameApplicationManager` script inherits from our base `ApplicationManager` class and provides a front facing API for developers to use in order to add new `StateMachines`, `Services`, and `EntityServices` to the application's bootstrapping routine. All of our core architectural dependencies live on `GameObjects`, and as such the `RootNameApplicationManager` is added as a script component to the `ApplicationManager` prefab, which lives at the root of our "Resources" folder. Let's take a closer look at the `RootNameApplicationManager` script and how it is used:

```csharp
using RootName.Core;
using UnityEngine;

// The RootNameApplicationManager script is a component of the root GameObject
// on the ApplicationManager prefab in Assets/Resources.
//
// This script contains references to the parent Transforms of the StateMachines,
// Services, EntityServices, and Controllers 
// These parent Transforms are used for controlling the timing of when
// their Unity Lifecycle methods are executed, and ensures that none of them will
// attempt to get Services and so on, or broadcast Message Events prior 
// to the completion of application bootstrapping.
namespace RootName.Runtime
{
    // This class is internal to the Core namespace and sealed to prevent
    // extension via inheritance.
    //
    // We inherit from ApplicationManager, which takes care of the logic for:
    // - Instantiation of this Singleton prior to the scene loading.
    // - Adding/removing/publishing Message Events.
    // - Adding/removing/getting StateMachines, Services, and EntityServices.
    // - Giving us access to various overrides related to application bootstrapping.
    internal sealed class RootNameApplicationManager : ApplicationManager
    {
        // All parent transforms used for individual StateMachines, Services, EntityServices,
        // and Controllers are organized under the "Management" header.
        //
        // For every StateMachine, Service, or EntityService created for the application:
        //
        // - Create a child GameObject under the associated ParentTransform in the prefab
        // - Serialize a reference to the GameObject's MonoBehaviour StateMachine,
        //   Service, or EntityService component into the editor.
        // - Drag and drop the child GameObject to the appropriate serialized field
        [Header("Management")]
        [SerializeField] private Transform statesParentTransform;
        
        [SerializeField] private Transform servicesParentTransform;
        
        [SerializeField] private Transform entityServicesParentTransform;

        // We use the controllersParentTransform purely for timing.
        // This prevents any Controllers on the ApplicationManager prefab
        // from interacting with a StateMachine, Service, or EntityService
        // before bootstrapping is complete.
        [SerializeField] private Transform controllersParentTransform;
        
        // We should keep our StateMachines, Services, EntityServices, and Controllers
        // organized under their own headers.
        //
        // Since we are referencing the MonoBehaviour script component from the GameObject
        // associated with our StateMachines, Services, and EntityServices, we
        // serialize their MonoBehaviour script components into the editor.
        [Header("State Machines")]
        [SerializeField] private ExampleStateMachine exampleStateMachine;
        
        [Header("Services")]
        [SerializeField] private ExampleService exampleService;
        
        [Header("Entity Services")]
        [SerializeField] private ExampleEntityService exampleEntityService;
        
        // Note that we do not need to serialize references to individual Controllers.
        
        // At runtime and after instantiation, we set the name of the instantiated
        // ApplicationManager prefab to be the same as this script component.
        // In our case, it will be name RootNameApplicationManager.
        protected override string GetApplicationName()
        {
            return nameof(RootNameApplicationManager);
        }

        // OnInitialized() is called after the singleton ApplicationManager
        // prefab has been instantiated into the scene. This is where
        // the bootstrapping happens.
        protected override void OnInitialized()
        {
            // We separate bootstrapping of individual StateMachines, Services,
            // and EntityServices into their own methods.
            InitializeApplicationStateMachines();
            InitializeApplicationServices();
            InitializeApplicationEntityServices();
            
            // Once bootstrapping is complete, we set the parent GameObjects
            // of the StateMachines, Services, EntityServices, and Controllers 
            // to active, which allows them to begin executing
            // their Unity Lifecycle methods.
            SetParentsActive();
        }

        // InitializeApplicationStateMachines() is where we add
        // StateMachines to the StateMachineManager so that we can
        // get them from the ApplicationManager by calling 
        // ApplicationManager.GetStateMachine<T>().
        protected override void InitializeApplicationStateMachines()
        {
            // When adding a StateMachine, Service, or EntityService to the associated Manager
            // within the ApplicationManager, we must bind the MonoBehaviour script component
            // to its associated interface. We also must specify that we are targeting the
            // GameObject associated with the MonoBehaviour script component.
            AddStateMachine<ApplicationStateMachine, IApplicationStateMachine>(applicationStateMachine);
        }
        
        // InitializeApplicationServices() is where we add
        // Services to the ServiceManager so that we can
        // get them from the ApplicationManager by calling 
        // ApplicationManager.GetService<T>().
        protected override void InitializeApplicationServices()
        {
            AddService<ExampleService, IExampleService>(exampleService);
        }
        
        // InitializeApplicationEntityServices() is where we add
        // EntityService to the EntityServiceManager so that we can
        // get them from the ApplicationManager by calling 
        // ApplicationManager.GetEntityService<T>().
        protected override void InitializeApplicationEntityServices()
        {
            AddEntityService<ExampleEntityService, IExampleEntityService>(exampleEntityService);
        }

        // SetParentsActive() is called after bootstrapping is complete.
        protected override void SetParentsActive()
        {
            statesParentTransform.gameObject.SetActive(true);
            servicesParentTransform.gameObject.SetActive(true);
            entityServicesParentTransform.gameObject.SetActive(true);
            controllersParentTransform.gameObject.SetActive(true);
        }
    }
}
```

# Conclusion

The `CoreArchitecture.md` document lays the technical foundation for building a robust, scalable, and flexible enterprise-level application. By exploring the mechanics behind core components such as `Message Events`, `StateMachines`, `Services`, `EntityServices`, and how these are managed by and retrieved from the `ApplicationManager`, developers can gather a greater understanding of how our core architecture works under the hood. This structured approach simplifies navigation and comprehension of the codebase, while fostering an environment that's conducive to collaborative development and rapid feature iteration and implementation. By leveraging the outlined architectural principles, application development can be accelerated, giving developers greater agility and delivering a seamless experience for end-users.