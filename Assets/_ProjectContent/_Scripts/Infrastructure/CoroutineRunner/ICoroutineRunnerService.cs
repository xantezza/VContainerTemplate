using System.Collections;
using UnityEngine;

namespace VContainerTemplate.Infrastructure.Services.CoroutineRunner
{
    public interface ICoroutineRunnerService
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
        void StopCoroutine(Coroutine coroutine);
    }
}