using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainerTemplate.Infrastructure.AssetReferenceProvider;
using VContainerTemplate.Infrastructure.Pooling;

namespace VContainerTemplate.UI.ModalPopups.Factory
{
    public class ModalPopupFactory : MonoBehaviour, IModalPopupFactory
    {
        [Inject] private IAssetReferenceProvider _assetReferenceProvider;
        [Inject] private IObjectResolver _objectResolver;
        private Transform _poolTransform;

        private Dictionary<Type, object> _pools = new();

        private AddressablesPool<T> GetPool<T>() where T : ModalPopup
        {
            var type = typeof(T);
            if (_pools.TryGetValue(type, out var poolObj))
            {
                return (AddressablesPool<T>)poolObj;
            }

            var reference = _assetReferenceProvider.ModalsAssetReferences.TypeToReference<T>();
            var pool = new AddressablesPool<T>(_objectResolver, reference, transform);
            _pools[type] = pool;
            return pool;
        }

        public async UniTask<T> Show<T>() where T : ModalPopup
        {
            var pool = GetPool<T>();
            var modalPopup = await pool.GetAsync(true);
            modalPopup.SetModalPopupFactory(this);
            modalPopup.Show();
            return modalPopup;
        }

        public async UniTask<T> Spawn<T>() where T : ModalPopup
        {
            var pool = GetPool<T>();
            var modalPopup = await pool.GetAsync(true);
            modalPopup.SetModalPopupFactory(this);
            return modalPopup;
        }

        public void ReturnToPool<T>(T modalPopup) where T : ModalPopup
        {
            var pool = GetPool<T>();
            pool.ReturnToPool(modalPopup);
        }
    }
}