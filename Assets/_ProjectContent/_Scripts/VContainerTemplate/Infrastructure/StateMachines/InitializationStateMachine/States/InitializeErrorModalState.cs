using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;
using VContainerTemplate.UI.ModalPopups;
using VContainerTemplate.UI.ModalPopups.Factory;

namespace VContainerTemplate.Infrastructure.StateMachines.Initialization.States
{
    [UsedImplicitly]
    public class InitializeErrorModalState : BaseInitializationState, IEnterableState, IDisposable
    {
        private readonly IModalPopupFactory _modalPopupFactory;

        [Inject]
        public InitializeErrorModalState(InitializationStateMachine stateMachine, IModalPopupFactory modalPopupFactory) : base(stateMachine)
        {
            _modalPopupFactory = modalPopupFactory;
        }

        public async UniTask Enter()
        {
            Application.logMessageReceived += OnApplicationMessageReceived;
            await _stateMachine.NextState();
        }

        private async void OnApplicationMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (type is LogType.Error or LogType.Exception)
            {
                var popup = await _modalPopupFactory.Spawn<ErrorModal>();
                popup.Init(condition, stacktrace);
                popup.Show();
            }
        }

        public void Dispose()
        {
            Application.logMessageReceived -= OnApplicationMessageReceived;
        }
    }
}