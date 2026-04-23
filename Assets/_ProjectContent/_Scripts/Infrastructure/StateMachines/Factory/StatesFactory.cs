using JetBrains.Annotations;
using VContainer;
using VContainerTemplate.Utilities.Extensions;

namespace VContainerTemplate.Infrastructure.StateMachines.Factory
{
    [UsedImplicitly]
    public class StatesFactory : IStatesFactory
    {
        private readonly IObjectResolver _objectResolver;

        public StatesFactory(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public TState Create<TState>(BaseStateMachine stateMachine) where TState : class, IState
        {
            return _objectResolver.Instantiate<TState>(Lifetime.Singleton, stateMachine);
        }
    }
}