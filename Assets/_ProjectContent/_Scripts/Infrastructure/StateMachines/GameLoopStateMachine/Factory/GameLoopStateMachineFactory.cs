using JetBrains.Annotations;
using VContainer;
using VContainerTemplate.Infrastructure.Services.Log;
using VContainerTemplate.Infrastructure.StateMachines.Factory;

namespace VContainerTemplate.Infrastructure.StateMachines.GameLoopStateMachine.Factory
{
    [UsedImplicitly]
    public class GameLoopStateMachineFactory : IGameLoopStateMachineFactory
    {
        private GameLoopStateMachine _stateMachine;
        private readonly IObjectResolver _objectResolver;

        [Inject]
        public GameLoopStateMachineFactory(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public GameLoopStateMachine GetFrom(object summoner)
        {
            _stateMachine ??= new GameLoopStateMachine(_objectResolver.Resolve<IStatesFactory>());

            Log.Trace($"Access to the {nameof(GameLoopStateMachine)} is obtained from {summoner}", LogTag.GameLoopStateMachine);
            return _stateMachine;
        }
    }
}