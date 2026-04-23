using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using VContainerTemplate.Infrastructure.Services.LoadingCurtain;
using VContainerTemplate.Infrastructure.Services.Log;

namespace VContainerTemplate.Infrastructure.Services.SceneLoading
{
    [UsedImplicitly]
    public class SceneLoaderService : ISceneLoaderService
    {
        private readonly ILoadingCurtainPresenter _loadingCurtainPresenter;
        private string _cachedSceneGUID;

        public SceneLoaderService(ILoadingCurtainPresenter loadingCurtainPresenter)
        {
            _loadingCurtainPresenter = loadingCurtainPresenter;
        }

        public async UniTask LoadScene(AssetReference nextSceneName, bool autoHideCurtain, bool allowReloadSameScene = false)
        {
            await LoadSceneByAddressables(nextSceneName, autoHideCurtain, allowReloadSameScene);
        }

        private async UniTask LoadSceneByAddressables(AssetReference nextScene, bool autoHideCurtain, bool allowReloadSameScene)
        {
            if (!allowReloadSameScene && _cachedSceneGUID == nextScene.AssetGUID)
            {
                Log.Log.Trace("Scene tried to be loaded from itself, loading ignored", LogTag.SceneLoader);
                return;
            }

            await _loadingCurtainPresenter.Show(0.3f);
            var waitNextScene = Addressables.LoadSceneAsync(nextScene);
            
            while (!waitNextScene.IsDone)
            {
                //_loadingCurtainService.SetProgress01(waitNextScene.PercentComplete);

                await UniTask.Yield();
            }
            
            Log.Log.Trace($"Loaded scene: {waitNextScene.Result.Scene.name} \n{nextScene.AssetGUID}", LogTag.SceneLoader);

            //await UniTask.WaitForSeconds(RemoteConfig.Infrastructure.FakeMinimalLoadTime);
            if (autoHideCurtain) await _loadingCurtainPresenter.Hide(0.3f);
        }
    }
}