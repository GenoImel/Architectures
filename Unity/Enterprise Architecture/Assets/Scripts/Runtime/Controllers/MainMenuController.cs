using System;
using RootName.Core;
using RootName.Runtime.States.ApplicationStates;
using UnityEngine;
using UnityEngine.UI;

namespace RootName.Runtime.Controllers
{
    internal sealed class MainMenuController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button loginButton;
        [SerializeField] private Button logoutButton;
        
        private IApplicationStateMachine applicationStateMachine;

        private void Awake()
        {
            applicationStateMachine = ApplicationManager.GetStateMachine<IApplicationStateMachine>();
        }

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void OnLoginButtonClicked()
        {
            applicationStateMachine.SetMainState();
        }

        private void OnLogoutButtonClicked()
        {
            applicationStateMachine.SetLogoutState();
        }

        private void AddListeners()
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
            logoutButton.onClick.AddListener(OnLogoutButtonClicked);
        }

        private void RemoveListeners()
        {
            loginButton.onClick.RemoveListener(OnLoginButtonClicked);
            logoutButton.onClick.RemoveListener(OnLogoutButtonClicked);
        }
    }
}