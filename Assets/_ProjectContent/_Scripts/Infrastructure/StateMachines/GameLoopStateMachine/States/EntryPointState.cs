using System;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainerTemplate.Infrastructure.Services.LoadingCurtain;
using VContainerTemplate.Infrastructure.StateMachines.InitializationStateMachine.Factory;

namespace VContainerTemplate.Infrastructure.StateMachines.GameLoopStateMachine.States
{
    public class EntryPointState : BaseGameLoopState, IEnterableState
    {
        private readonly IInitializationStateMachineFactory _initializationStateMachineFactory;
        private readonly ILoadingCurtainPresenter _loadingCurtainPresenter;

        [Inject]
        public EntryPointState(
            GameLoopStateMachine stateMachine, 
            IInitializationStateMachineFactory initializationStateMachineFactory, 
            ILoadingCurtainPresenter loadingCurtainPresenter
            ) : base(stateMachine)
        {
            _loadingCurtainPresenter = loadingCurtainPresenter;
            _initializationStateMachineFactory = initializationStateMachineFactory;
        }

        public async UniTask Enter()
        {
            _loadingCurtainPresenter.ForceShow();
            await ToNextState();
        }

        private async UniTask ToNextState()
        {
            await _initializationStateMachineFactory.GetFrom(this).NextState();
        }
    }
}