using Cysharp.Threading.Tasks;
using VContainer;

namespace VContainerTemplate.Infrastructure.StateMachines.GameLoop
{
    public abstract class BaseGameLoopState : IState
    {
        protected readonly GameLoopStateMachine _stateMachine;

        [Inject]
        protected BaseGameLoopState(GameLoopStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public virtual UniTask Exit()
        {
            return default;
        }
    }
}