using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using VContainer;
using VContainerTemplate.Infrastructure.Services.Saving;

namespace VContainerTemplate.Infrastructure.StateMachines.InitializationStateMachine.States
{
    [UsedImplicitly]
    public class InitializeSaveServiceState : BaseInitializationState, IEnterableState
    {
        private readonly ISaveService _saveService;

        [Inject]
        public InitializeSaveServiceState(InitializationStateMachine gameLoopStateMachine, ISaveService saveService) : base(gameLoopStateMachine)
        {
            _saveService = saveService;
        }

        public async UniTask Enter()
        {
            _saveService.LoadSaveFile();
            await ToNextState();
        }

        private async UniTask ToNextState()
        {
            await _stateMachine.NextState();
        }
    }
}