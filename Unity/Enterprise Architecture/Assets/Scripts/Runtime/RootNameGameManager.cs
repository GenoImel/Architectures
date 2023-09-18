using RootName.Core;
using UnityEngine;

namespace RootName.Runtime
{
    internal sealed class RootNameGameManager : GameManager
    {
        [Header("Management")] 
        [SerializeField] private Transform monoSystemsParentTransform;

        [SerializeField] private Transform controllersParentTransform;

        protected override string GetApplicationName()
        {
            return nameof(RootNameGameManager);
        }

        protected override void OnInitialized()
        {
            monoSystemsParentTransform.gameObject.SetActive(true);
            controllersParentTransform.gameObject.SetActive(true);
        }
    }
}