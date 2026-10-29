using JetBrains.Annotations;
using VContainerTemplate.Infrastructure.Logger;
using VContainerTemplate.Infrastructure.StateMachines.Factory;
using VContainerTemplate.Infrastructure.StateMachines.Initialization.States;

namespace VContainerTemplate.Infrastructure.StateMachines.Initialization
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
#if !(SKIP_PRIVACY_MODAL && DEV)
            RegisterState(statesFactory.Create<InitializePrivacyPolicyModalState>(this));
#endif
            RegisterState(statesFactory.Create<InitializationFinalizerState>(this));
        }
    }
}