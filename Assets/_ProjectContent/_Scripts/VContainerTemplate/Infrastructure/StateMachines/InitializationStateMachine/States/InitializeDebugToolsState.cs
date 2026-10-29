using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainerTemplate.Infrastructure.AssetReferenceProvider;

namespace VContainerTemplate.Infrastructure.StateMachines.Initialization.States
{
    public class InitializeDebugToolsState : BaseInitializationState, IEnterableState
    {
        private readonly IAssetReferenceProvider _assetReferenceProvider;

        [Inject]
        public InitializeDebugToolsState(InitializationStateMachine stateMachine, IAssetReferenceProvider assetReferenceProvider) : base(stateMachine)
        {
            _assetReferenceProvider = assetReferenceProvider;
        }

        public async UniTask Enter()
        {
            var debugRoot = await _assetReferenceProvider.DebugRootAssetReference.InstantiateAsync();
            Object.DontDestroyOnLoad(debugRoot);
            await _stateMachine.NextState();
        }
    }
}