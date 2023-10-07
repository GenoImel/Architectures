using System;
using RootName.Core.EntityServices;
using RootName.Core.Messages;
using RootName.Core.Services;
using RootName.Core.StateMachines;
using UnityEngine;

namespace RootName.Core
{
   internal abstract class ApplicationManager : MonoBehaviour
    {
        private const string ApplicationManagerPrefabPath = "ApplicationManager";
        private static ApplicationManager instance;
        
        private readonly MessageManager messageManager = new ();
        private readonly StateMachineManager stateMachineManager = new();
        private readonly ServiceManager serviceManager = new ();
        private readonly EntityServiceManager entityServiceManager = new ();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (instance)
            {
                return;
            }

            var applicationManagerPrefab = Resources.Load<ApplicationManager>(ApplicationManagerPrefabPath);
            var applicationManager = Instantiate(applicationManagerPrefab);

            applicationManager.name = applicationManager.GetApplicationName();
            
            DontDestroyOnLoad(applicationManager);

            instance = applicationManager;

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
        /// Returns an existing state in the application.
        /// </returns>
        public static TStateMachine GetStateMachine<TStateMachine>()
        {
            return instance.stateMachineManager.GetStateMachine<TStateMachine>();
        }
        
        /// <summary>
        /// Adds a State to the application.
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