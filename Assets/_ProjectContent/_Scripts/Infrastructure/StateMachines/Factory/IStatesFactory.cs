namespace VContainerTemplate.Infrastructure.StateMachines.Factory
{
    public interface IStatesFactory
    {
        TState Create<TState>(BaseStateMachine stateMachine) where TState : class, IState;
    }
}