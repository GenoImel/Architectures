using RootName.Core;
using RootName.Runtime.States.ApplicationStates;
using UnityEngine;

namespace RootName.Runtime
{
    internal sealed class RootNameApplicationManager : ApplicationManager
    {
        [Header("Management")]
        [SerializeField] private Transform statesParentTransform;

        [SerializeField] private Transform servicesParentTransform;

        [SerializeField] private Transform entityServicesParentTransform;

        [SerializeField] private Transform controllersParentTransform;
        
        [Header("States")]
        [SerializeField] private ApplicationStateMachine applicationStateMachine;

        protected override string GetApplicationName()
        {
            return nameof(RootNameApplicationManager);
        }

        protected override void OnInitialized()
        {
            InitializeApplicationStateMachines();
            InitializeApplicationServices();
            InitializeApplicationEntityServices();
            
            SetParentsActive();
        }

        protected override void InitializeApplicationStateMachines()
        {
            AddStateMachine<ApplicationStateMachine, IApplicationStateMachine>(applicationStateMachine);
        }
        
        protected override void InitializeApplicationServices()
        {
            
        }
        
        protected override void InitializeApplicationEntityServices()
        {
            
        }

        protected override void SetParentsActive()
        {
            statesParentTransform.gameObject.SetActive(true);
            servicesParentTransform.gameObject.SetActive(true);
            entityServicesParentTransform.gameObject.SetActive(true);
            controllersParentTransform.gameObject.SetActive(true);
        }
    }
}