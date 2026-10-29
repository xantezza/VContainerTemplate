using Cysharp.Threading.Tasks;
using VContainer;
using VContainerTemplate.Configs.RemoteConfig;
using VContainerTemplate.Infrastructure.DefaultConfigProvider;

namespace VContainerTemplate.Infrastructure.StateMachines.Initialization.States
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