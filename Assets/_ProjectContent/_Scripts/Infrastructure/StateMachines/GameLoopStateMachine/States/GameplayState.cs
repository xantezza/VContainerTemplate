using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainerTemplate.Infrastructure.Providers.AssetReferenceProvider;
using VContainerTemplate.Infrastructure.Services.Analytics;
using VContainerTemplate.Infrastructure.Services.Saving;
using VContainerTemplate.Infrastructure.Services.SceneLoading;

namespace VContainerTemplate.Infrastructure.StateMachines.GameLoopStateMachine.States
{
    public class GameplayState : BaseGameLoopState, IEnterableState
    {
        private readonly ISaveService _saveService;
        private readonly ISceneLoaderService _sceneLoaderService;
        private readonly IAssetReferenceProvider _assetReferenceProvider;
        private readonly IAnalyticsService _analyticsService;

        [Inject]
        public GameplayState(
            GameLoopStateMachine gameLoopStateMachine,
            ISaveService saveService,
            ISceneLoaderService sceneLoaderService,
            IAssetReferenceProvider assetReferenceProvider,
            IAnalyticsService analyticsService
        ) : base(gameLoopStateMachine)
        {
            _analyticsService = analyticsService;
            _assetReferenceProvider = assetReferenceProvider;
            _sceneLoaderService = sceneLoaderService;
            _saveService = saveService;
        }

        public async UniTask Enter()
        {
            await _sceneLoaderService.LoadScene(_assetReferenceProvider.GamePlayScene, true);
        }

        public override async UniTask Exit()
        {
            _analyticsService.SendEvent(AnalyticsNames.AverageFPS, Time.frameCount / Time.realtimeSinceStartup);
            await _saveService.StoreSaveFile();
        }
    }
}