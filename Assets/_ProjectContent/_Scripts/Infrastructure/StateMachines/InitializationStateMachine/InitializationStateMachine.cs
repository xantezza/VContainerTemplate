using JetBrains.Annotations;
using VContainerTemplate.Infrastructure.Services.Log;
using VContainerTemplate.Infrastructure.StateMachines.Factory;
using VContainerTemplate.Infrastructure.StateMachines.InitializationStateMachine.States;

namespace VContainerTemplate.Infrastructure.StateMachines.InitializationStateMachine
{
    [UsedImplicitly]
    public class InitializationStateMachine : SequentialStateMachine
    {
        protected override LogTag LogTag => LogTag.InitializationStateMachine;

        public InitializationStateMachine(IStatesFactory statesFactory)
        {
#if DEV
            RegisterState(statesFactory.Create<InitializeDebugToolsState>(this));
#else
            RegisterState(statesFactory.Create<InitializeErrorModalState>(this));
#endif
            RegisterState(statesFactory.Create<InitializeDefaultConfigState>(this));
            RegisterState(statesFactory.Create<InitializeUnityServicesState>(this));
            RegisterState(statesFactory.Create<InitializeSaveServiceState>(this));
            RegisterState(statesFactory.Create<InitializePrivacyPolicyModalState>(this));
            RegisterState(statesFactory.Create<InitializationFinalizerState>(this));
        }
    }
}