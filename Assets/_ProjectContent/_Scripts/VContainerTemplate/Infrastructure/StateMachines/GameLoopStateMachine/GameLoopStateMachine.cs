using System;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainerTemplate.Infrastructure.Logger;
using VContainerTemplate.Infrastructure.StateMachines.Factory;
using VContainerTemplate.Infrastructure.StateMachines.GameLoop.States;

namespace VContainerTemplate.Infrastructure.StateMachines.GameLoop
{
    public enum TargetGameLoopState
    {
        Menu = 2,
        Gameplay = 3
    }
    
    public class GameLoopStateMachine : BaseStateMachine
    {
        protected override LogTag LogTag => LogTag.GameLoopStateMachine;

        [Inject]
        public GameLoopStateMachine(IStatesFactory statesFactory)
        {
            RegisterState(statesFactory.Create<MenuState>(this));
            RegisterState(statesFactory.Create<GameplayState>(this));
        }
        
        public new async UniTask Enter<TState>() where TState : class, IState, IEnterableState
        {
            await base.Enter<TState>();
        }

        public async UniTask Enter(TargetGameLoopState targetGameLoopState)
        {
            switch (targetGameLoopState)
            {
                case TargetGameLoopState.Menu:
                    await base.Enter<MenuState>();
                    break;
                case TargetGameLoopState.Gameplay:
                    await base.Enter<GameplayState>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetGameLoopState), targetGameLoopState, null);
            }
        }

    }
}