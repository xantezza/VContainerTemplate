using System;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainerTemplate.Infrastructure.Services.Log;
using VContainerTemplate.Infrastructure.StateMachines.Factory;
using VContainerTemplate.Infrastructure.StateMachines.GameLoopStateMachine.States;

namespace VContainerTemplate.Infrastructure.StateMachines.GameLoopStateMachine
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
            RegisterState(statesFactory.Create<EntryPointState>(this));
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