namespace VContainerTemplate.Infrastructure.StateMachines.InitializationStateMachine.Factory
{
    public interface IInitializationStateMachineFactory
    {
        InitializationStateMachine GetFrom(object summoner);
    }
}