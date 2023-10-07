using RootName.Core;
using UnityEngine;

namespace RootName.Runtime
{
    internal sealed class RootNameGameManager : GameManager
    {
        [Header("Management")]
        [SerializeField] private Transform stateMachinesParentTransform;

        [SerializeField] private Transform monoSystemsParentTransform;

        [SerializeField] private Transform entitySystemsParentTransform;

        [SerializeField] private Transform controllersParentTransform;

        protected override string GetGameName()
        {
            return nameof(RootNameGameManager);
        }

        protected override void OnInitialized()
        {
            InitializeGameStateMachines();
            InitializeGameServices();
            InitializeGameEntityServices();
            
            SetParentsActive();
        }

        protected override void InitializeGameStateMachines()
        {
        }
        
        protected override void InitializeGameServices()
        {
        }
        
        protected override void InitializeGameEntityServices()
        {
        }

        protected override void SetParentsActive()
        {
            stateMachinesParentTransform.gameObject.SetActive(true);
            monoSystemsParentTransform.gameObject.SetActive(true);
            entitySystemsParentTransform.gameObject.SetActive(true);
            controllersParentTransform.gameObject.SetActive(true);
        }
    }
}