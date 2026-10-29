using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VContainerTemplate.Utilities.MonoBehaviours
{
    [RequireComponent(typeof(TrailRenderer))]
    public class TrailEmitHelper : MonoBehaviour
    {
        private TrailRenderer _trailRenderer;

        private void OnDisable()
        {
            _trailRenderer ??= GetComponent<TrailRenderer>();
            _trailRenderer.Clear();
            _trailRenderer.enabled = false;
        }

        private async void OnEnable()
        {
            await UniTask.DelayFrame(5);
            _trailRenderer ??= GetComponent<TrailRenderer>();
            _trailRenderer.enabled = true;
        }
    }
}