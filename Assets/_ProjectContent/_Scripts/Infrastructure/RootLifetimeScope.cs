using UnityEngine;
using VContainer;
using VContainer.Unity;
using VContainerTemplate.Infrastructure.Audio;
using VContainerTemplate.Infrastructure.Providers.AssetReferenceProvider;
using VContainerTemplate.Infrastructure.Providers.DefaultConfigProvider;
using VContainerTemplate.Infrastructure.Services.Analytics;
using VContainerTemplate.Infrastructure.Services.CoroutineRunner;
using VContainerTemplate.Infrastructure.Services.LoadingCurtain;
using VContainerTemplate.Infrastructure.Services.Saving;
using VContainerTemplate.Infrastructure.Services.SceneLoading;
using VContainerTemplate.Infrastructure.Services.Settings;
using VContainerTemplate.Infrastructure.Services.Tutorial;
using VContainerTemplate.Infrastructure.Services.Windows;
using VContainerTemplate.Infrastructure.StateMachines.Factory;
using VContainerTemplate.Infrastructure.StateMachines.GameLoopStateMachine.Factory;
using VContainerTemplate.Infrastructure.StateMachines.InitializationStateMachine.Factory;
using VContainerTemplate.UI.ModalPopups;
using VContainerTemplate.Utilities.Extensions;
using VContainerTemplate.Utilities.MonoBehaviours;

namespace VContainerTemplate.Infrastructure
{
    public class RootLifetimeScope : AutoLifetimeScope
    {
        [SerializeField] private ModalPopupFactory _modalPopupFactory;

        [SerializeField] private AssetReferenceProvider _assetReferenceProvider;
        [SerializeField] private AudioProvider _audioProvider;
        [SerializeField] private CachedDefaultUnityRemoteConfigProvider _cachedDefaultUnityRemoteConfigProvider;

        [SerializeField] private LoadingCurtainView _loadingCurtainView;
        [SerializeField] private AudioService _audioService;
        [SerializeField] private TutorialService _tutorialService;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterFactories(builder);
            RegisterProviders(builder);
            RegisterServices(builder);
        }

        private void RegisterFactories(IContainerBuilder builder)
        {
            builder.Register(_modalPopupFactory);
            builder.Register<StatesFactory>();
            builder.Register<GameLoopStateMachineFactory>();
            builder.Register<InitializationStateMachineFactory>();
        }

        private void RegisterProviders(IContainerBuilder builder)
        {
            builder.Register(_assetReferenceProvider);
            builder.Register(_audioProvider);
            builder.Register(_cachedDefaultUnityRemoteConfigProvider);
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register(new LoadingCurtainPresenter(_loadingCurtainView));
            builder.Register(_audioService);
            builder.Register(_tutorialService);
            
            builder.Register<UnityAnalyticsService>();
            builder.Register<CoroutineRunnerService>();
            builder.Register<SceneLoaderService>();
            builder.Register<WindowsService>();
            builder.Register<SettingsService>();
#if DEV
            // Readable
            builder.Register<JsonSaveService>();
#else
            // Encrypted
             builder.Register<BinarySaveService>();
#endif
        }
    }
}