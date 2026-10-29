using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.RemoteConfig;
using VContainer;
using VContainerTemplate.Configs.RemoteConfig;
using VContainerTemplate.Infrastructure.Logger;

namespace VContainerTemplate.Infrastructure.StateMachines.Initialization.States
{
    public class InitializeUnityServicesState : BaseInitializationState, IEnterableState
    {
        public static bool IsInitialized { get; private set; }

        [Inject]
        public InitializeUnityServicesState(InitializationStateMachine stateMachine) : base(stateMachine)
        {
        }

        public async UniTask Enter()
        {
            await InitializeRemoteSettings();
        }

        private async UniTask InitializeRemoteSettings()
        {
            if (IsInitialized)
            {
                await _stateMachine.NextState();
                return;
            }

#if PLATFORM_WEBGL && !UNITY_EDITOR
            InitializeUnityServices().Forget();
#else
            InitializeUnityServices().Forget();
            await _stateMachine.NextState();
#endif

            RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
            Log.Trace("Fetch Configs", LogTag.UnityServices);
            RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
        }

        private async UniTaskVoid InitializeUnityServices()
        {
#if DEV
            var environmentName = "production" /*"dev"*/;
            var options = new InitializationOptions().SetEnvironmentName(environmentName);
            Log.Trace($"UnityServices.InitializeAsync with environment: {environmentName}", LogTag.UnityServices);
#else
            var options = new InitializationOptions().SetEnvironmentName("production");
#endif

            await UnityServices.InitializeAsync(options);

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Log.Trace("Start of SignInAnonymouslyAsync", LogTag.UnityServices);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        private void ApplyRemoteSettings(ConfigResponse configResponse)
        {
            Log.Trace($"Request Origin: {configResponse.requestOrigin}", LogTag.UnityServices);

            if (configResponse.requestOrigin == ConfigOrigin.Default) return;

            RemoteConfig.InitializeByRemote(RemoteConfigService.Instance.appConfig.config);

            IsInitialized = true;
        }

        private struct userAttributes
        {
        }

        private struct appAttributes
        {
        }
    }
}