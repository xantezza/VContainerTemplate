using System.Collections;
using UnityEngine;

namespace VContainerTemplate.Infrastructure.CoroutineRunner
{
    public interface ICoroutineRunnerService
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
        void StopCoroutine(Coroutine coroutine);
    }
}