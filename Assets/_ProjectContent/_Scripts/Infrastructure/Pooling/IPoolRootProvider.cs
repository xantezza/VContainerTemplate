using UnityEngine;

namespace VContainerTemplate.Infrastructure.Services.Pooling
{
    public interface IPoolRootProvider
    {
        public Transform PoolRoot { get; }
    }
}