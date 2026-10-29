using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace VContainerTemplate.Infrastructure.Pooling
{
    public class Pool<T> where T : Component
    {
        private readonly T _reference;
        private readonly Transform _parent;
        private Transform _pooledObjectsParent;
        private readonly Stack<T> _pool = new();
        private int _activeObjectsCount;
        private readonly IObjectResolver _objectResolver;
        public int ActiveObjectsCount => _activeObjectsCount;

        public Pool(T reference, IObjectResolver objectResolver, Transform parent = null)
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
                item.gameObject.transform.SetParent(_parent, false);
                _activeObjectsCount++;
                return item;
            }

            var handle = Object.InstantiateAsync(_reference, _parent);
            await handle;
            var instance = handle.Result;
            _activeObjectsCount++;
            if (inject) _objectResolver.InjectGameObject(instance[0].gameObject);
            return instance[0];
        }

        public T Get(bool inject = false)
        {
            if (_pool.Count > 0)
            {
                var item = _pool.Pop();
                item.gameObject.transform.SetParent(_parent, false);
                _activeObjectsCount++;
                return item;
            }

            var instance = Object.Instantiate(_reference, _parent);
            _activeObjectsCount++;
            if (inject) _objectResolver.InjectGameObject(instance.gameObject);
            return instance;
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