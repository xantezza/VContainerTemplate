using UnityEngine.AddressableAssets;

namespace VContainerTemplate.Infrastructure.Providers.AssetReferenceProvider
{
    public interface IAssetReferenceProvider
    {
        ModalsAssetReferences ModalsAssetReferences { get; }
        AssetReferenceGameObject DebugRootAssetReference { get; }
        AssetReference MenuScene { get; }
        AssetReference GamePlayScene { get; }
    }
}