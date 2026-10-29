using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using VContainer.Unity;
using VContainerTemplate.Infrastructure.Logger;

namespace VContainerTemplate.Infrastructure.Pooling
{
    public class AddressablesPool<T> where T : Component
    {
        private readonly AssetReference _reference;
        private readonly Transform _parent;
        private Transform _pooledObjectsParent;
        private readonly Stack<T> _pool = new();
        private readonly IObjectResolver _objectResolver;
        private int _activeObjectsCount;
        public int ActiveObjectsCount => _activeObjectsCount;

        public AddressablesPool(IObjectResolver objectResolver, AssetReferenceGameObject reference, Transform parent = null)
        {
            _objectResolver = objectResolver;
            _reference = reference;
            _parent = parent;
        }

        private void EnsurePooledObjectsParent()
        {
            if (_pooledObjectsParent != null)
                return;

            _pooledObjectsParent = new GameObject("Pool Root").transform;
            _pooledObjectsParent.parent = _parent;
        }
        
        public async Task<T> GetAsync(bool inject = false)
        {
            if (_pool.Count > 0)
            {
                var item = _pool.Pop();
                _activeObjectsCount++;
                item.gameObject.transform.SetParent(_parent, false);
                return item;
            }

            var handle = _reference.LoadAssetAsync<GameObject>();
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var instance = Object.Instantiate(handle.Result, _parent);
                instance.SetActive(false);
                var component = instance.GetComponent<T>();
                if (component != null)
                {
                    _activeObjectsCount++;
                    if (inject) _objectResolver.InjectGameObject(component.gameObject);
                    return component;
                }
                Log.Error($"Object dont have {typeof(T)}");
                _reference.ReleaseInstance(instance);
                return null;
            }

            Log.Error("Cant get object from Addressables");
            return null;
        }

        public void ReturnToPool(T item)
        {
            if (item == null)
                return;

            _activeObjectsCount--;
            item.gameObject.SetActive(false);
            EnsurePooledObjectsParent();
            item.gameObject.transform.SetParent(_pooledObjectsParent, false);
            _pool.Push(item);
        }

        public void Clear()
        {
            while (_pool.Count > 0)
            {
                var item = _pool.Pop();
                if (item != null)
                {
                    Object.Destroy(item.gameObject);
                }
            }

            if (_pooledObjectsParent != null)
            {
                Object.Destroy(_pooledObjectsParent.gameObject);
                _pooledObjectsParent = null;
            }
            
            _activeObjectsCount = 0;
        }
    }
}