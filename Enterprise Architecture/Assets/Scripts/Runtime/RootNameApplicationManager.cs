using RootName.Core;
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