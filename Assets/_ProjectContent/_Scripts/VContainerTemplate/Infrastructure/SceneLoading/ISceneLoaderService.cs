using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace VContainerTemplate.Infrastructure.SceneLoading
{
    public interface ISceneLoaderService
    {
        UniTask LoadScene(AssetReference nextSceneName, bool autoHideCurtain, bool allowReloadSameScene = false);
    }
}