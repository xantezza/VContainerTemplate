using JetBrains.Annotations;
using VContainer;
using VContainerTemplate.Infrastructure.Services.Log;
using VContainerTemplate.Infrastructure.StateMachines.Factory;

namespace VContainerTemplate.Infrastructure.StateMachines.InitializationStateMachine.Factory
{
    [UsedImplicitly]
    public class InitializationStateMachineFactory : IInitializationStateMachineFactory
    {
        private InitializationStateMachine _stateMachine;
        private readonly IObjectResolver _objectResolver;

        [Inject]
        public InitializationStateMachineFactory(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public InitializationStateMachine GetFrom(object summoner)
        {
            _stateMachine ??= new InitializationStateMachine(_objectResolver.Resolve<IStatesFactory>());
            Log.Trace($"Access to the {nameof(InitializationStateMachine)} is obtained from {summoner}", LogTag.InitializationStateMachine);
            return _stateMachine;
        }
    }
}