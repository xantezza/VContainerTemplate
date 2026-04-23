namespace VContainerTemplate.Infrastructure.StateMachines.GameLoopStateMachine.Factory
{
    public interface IGameLoopStateMachineFactory
    {
        GameLoopStateMachine GetFrom(object summoner);
    }
}