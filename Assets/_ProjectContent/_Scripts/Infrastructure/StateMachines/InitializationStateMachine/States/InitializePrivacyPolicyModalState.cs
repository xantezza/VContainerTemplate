using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UniRx;
using UnityEngine.UnityConsent;
using VContainer;
using VContainerTemplate.Infrastructure.Services.Saving;
using VContainerTemplate.UI.ModalPopups;

namespace VContainerTemplate.Infrastructure.StateMachines.InitializationStateMachine.States
{
    [UsedImplicitly]
    public class InitializePrivacyPolicyModalState : BaseInitializationState, IDataSaveable<InitializePrivacyPolicyModalState.Save>, IEnterableState, IDisposable
    {
        private const bool DEV_CONSENT = false;

        [Serializable]
        public class Save
        {
            public bool ConsentGiven = false;
        }

        public SaveKey SaveKey => SaveKey.PrivacyPolicyState;
        public Save SaveData { get; private set; }

        private readonly ISaveService _saveService;
        private readonly IModalPopupFactory _modalPopupFactory;
        private IDisposable _subscription;

        [Inject]
        public InitializePrivacyPolicyModalState(InitializationStateMachine stateMachine, IModalPopupFactory modalPopupFactory, ISaveService saveService) : base(stateMachine)
        {
            _modalPopupFactory = modalPopupFactory;
            _saveService = saveService;
        }

        public async UniTask Enter()
        {
            SaveData = _saveService.Load(this) ?? new Save();
            _saveService.AddToSaveables(this);

#if DEV
            SaveData.ConsentGiven = DEV_CONSENT;
#endif
            if (SaveData.ConsentGiven)
            {
                if (InitializeUnityServicesState.IsInitialized) EndUserConsent.SetConsentState(new ConsentState(){AnalyticsIntent = ConsentStatus.Granted});
                await _stateMachine.NextState();
            }
            else
            {
                _subscription = (await _modalPopupFactory.Show<PrivacyPolicyModal>()).OnInteract.Subscribe(_ => OnInteract());
            }
        }

        private async void OnInteract()
        {
            SaveData.ConsentGiven = true;
            if (InitializeUnityServicesState.IsInitialized) EndUserConsent.SetConsentState(new ConsentState(){AnalyticsIntent = ConsentStatus.Granted});
            await _stateMachine.NextState();
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}