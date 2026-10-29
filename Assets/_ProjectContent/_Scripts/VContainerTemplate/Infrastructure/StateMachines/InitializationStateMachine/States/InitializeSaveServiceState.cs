using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using VContainer;
using VContainerTemplate.Infrastructure.Saving;
using VContainerTemplate.Infrastructure.Settings;

namespace VContainerTemplate.Infrastructure.StateMachines.Initialization.States
{
    [UsedImplicitly]
    public class InitializeSaveServiceState : BaseInitializationState, IEnterableState
    {
        private readonly ISaveService _saveService;
        private readonly ISettingsService _settingsService;

        [Inject]
        public InitializeSaveServiceState(InitializationStateMachine gameLoopStateMachine, ISaveService saveService, ISettingsService settingsService) : base(gameLoopStateMachine)
        {
            _settingsService = settingsService;
            _saveService = saveService;
        }

        public async UniTask Enter()
        {
            _saveService.LoadSaveFile();
            InitializeServices();
            await _stateMachine.NextState();
        }

        public void InitializeServices()
        {
            _settingsService.LoadSave();
        }
    }
}