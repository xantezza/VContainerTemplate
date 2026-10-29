using UnityEngine;
using VContainer;
using VContainerTemplate.Infrastructure.Analytics;
using VContainerTemplate.Infrastructure.Audio;
using VContainerTemplate.Infrastructure.CoroutineRunner;
using VContainerTemplate.Infrastructure.DefaultConfigProvider;
using VContainerTemplate.Infrastructure.LoadingCurtain;
using VContainerTemplate.Infrastructure.Saving;
using VContainerTemplate.Infrastructure.SceneLoading;
using VContainerTemplate.Infrastructure.Settings;
using VContainerTemplate.Infrastructure.StateMachines.Factory;
using VContainerTemplate.Infrastructure.StateMachines.GameLoop;
using VContainerTemplate.Infrastructure.StateMachines.Initialization;
using VContainerTemplate.Infrastructure.Windows;
using VContainerTemplate.UI.ModalPopups.Factory;
using VContainerTemplate.UI.Tutorial;
using VContainerTemplate.Utilities.Extensions;
using VContainerTemplate.Utilities.MonoBehaviours;

namespace VContainerTemplate.Infrastructure
{
    public class RootLifetimeScope : AutoLifetimeScope
    {
        [SerializeField] private ModalPopupFactory _modalPopupFactory;

        [SerializeField] private AssetReferenceProvider.AssetReferenceProvider _assetReferenceProvider;
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
            builder.RegisterInterfaces(_modalPopupFactory);
            builder.RegisterInterfaces<StatesFactory>();
            builder.Register<GameLoopStateMachine>(Lifetime.Singleton);
            builder.Register<InitializationStateMachine>(Lifetime.Singleton);
        }

        private void RegisterProviders(IContainerBuilder builder)
        {
            builder.RegisterInterfaces(_assetReferenceProvider);
            builder.RegisterInterfaces(_audioProvider);
            builder.RegisterInterfaces(_cachedDefaultUnityRemoteConfigProvider);
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.RegisterInterfaces(new LoadingCurtainPresenter(_loadingCurtainView));
            builder.RegisterInterfaces(_audioService);
            builder.RegisterInterfaces(_tutorialService);
            
            builder.RegisterInterfaces<UnityAnalyticsService>();
            builder.RegisterInterfaces<CoroutineRunnerService>();
            builder.RegisterInterfaces<SceneLoaderService>();
            builder.RegisterInterfaces<WindowsService>();
            builder.RegisterInterfaces<SettingsService>();
#if DEV
            // Readable
            builder.RegisterInterfaces<JsonSaveService>();
#else
            // Encrypted
             builder.RegisterInterfaces<BinarySaveService>();
#endif
        }
    }
}