using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;
using VContainerTemplate.Configs.RemoteConfig;
using VContainerTemplate.Infrastructure.Providers.DefaultConfigProvider;

namespace VContainerTemplate.Infrastructure.StateMachines.InitializationStateMachine.States
{
    public class InitializeDefaultConfigState : BaseInitializationState, IEnterableState
    {
        private readonly IDefaultConfigProvider _defaultConfigProvider;

        [Inject]
        public InitializeDefaultConfigState(
            InitializationStateMachine gameLoopStateMachine,
            IDefaultConfigProvider defaultConfigProvider
        ) : base(gameLoopStateMachine)
        {
            _defaultConfigProvider = defaultConfigProvider;
        }

        public async UniTask Enter()
        {
            RemoteConfig.InitializeByDefault(_defaultConfigProvider.CachedConfig);
            await _stateMachine.NextState();
        }
    }
}