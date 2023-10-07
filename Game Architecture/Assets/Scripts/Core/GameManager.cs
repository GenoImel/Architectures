using System;
using RootName.Core.EntitySystems;
using RootName.Core.Messages;
using RootName.Core.MonoSystems;
using RootName.Core.States;
using UnityEngine;

namespace RootName.Core
{
   internal abstract class GameManager : MonoBehaviour
    {
        private const string GameManagerPrefabPath = "GameManager";
        private static GameManager instance;
        
        private readonly MessageManager messageManager = new ();
        private readonly StateMachineManager stateMachineManager = new();
        private readonly MonoSystemManager monoSystemManager = new ();
        private readonly EntitySystemManager entitySystemManager = new ();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (instance)
            {
                return;
            }

            var gameManagerPrefab = Resources.Load<GameManager>(GameManagerPrefabPath);
            var gameManager = Instantiate(gameManagerPrefab);

            gameManager.name = gameManager.GetGameName();
            
            DontDestroyOnLoad(gameManager);

            instance = gameManager;

            gameManager.OnInitialized();
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
        /// Returns an existing state in the game.
        /// </returns>
        public static TStateMachine GetStateMachine<TStateMachine>()
        {
            return instance.stateMachineManager.GetStateMachine<TStateMachine>();
        }
        
        /// <summary>
        /// Adds a State to the game.
        /// </summary>
        protected void AddStateMachine<TStateMachine, TBindTo>(TStateMachine stateMachine) 
            where TStateMachine : IStateMachine, TBindTo
        {
            stateMachineManager.AddStateMachine<TStateMachine, TBindTo>(stateMachine);
        }

        /// <returns>
        /// Returns an existing Service in the game.
        /// </returns>
        public static TMonoSystem GetMonoSystem<TMonoSystem>()
        {
            return instance.monoSystemManager.GetMonoSystem<TMonoSystem>();
        }

        /// <summary>
        /// Adds a Service to the game.
        /// </summary>
        protected void AddMonoSystem<TMonoSystem, TBindTo>(TMonoSystem monoSystem) 
            where TMonoSystem : IMonoSystem, TBindTo
        {
            monoSystemManager.AddMonoSystem<TMonoSystem, TBindTo>(monoSystem);
        }

        /// <returns>
        ///Returns an existing Entity Service in the game.
        /// </returns>
        public static TEntitySystem GetEntitySystem<TEntitySystem>()
        {
            return instance.entitySystemManager.GetEntitySystem<TEntitySystem>();
        }

        /// <summary>
        /// Adds an Entity Service to the game.
        /// </summary>
        protected void AddEntityService<TEntitySystem, TBindTo>(TEntitySystem entitySystem)
            where TEntitySystem : IEntitySystem, TBindTo
        {
            entitySystemManager.AddEntitySystem<TEntitySystem, TBindTo>(entitySystem);
        }
        
        /// <returns>
        /// Name of the game or game.
        /// </returns>
        protected abstract string GetGameName();

        /// <summary>
        /// Called when the game manager is initialized.
        /// </summary>
        protected abstract void OnInitialized();

        /// <summary>
        /// Called when bootstrapping game state machines.
        /// </summary>
        protected abstract void InitializeGameStateMachines();
        
        /// <summary>
        /// Called when bootstrapping game services.
        /// </summary>
        protected abstract void InitializeGameServices();
        
        /// <summary>
        /// Called when bootstrapping game entity services.
        /// </summary>
        protected abstract void InitializeGameEntityServices();

        /// <summary>
        /// Called after bootstrapping complete.
        /// Sets all parent transforms active.
        /// </summary>
        protected abstract void SetParentsActive();
    }
}