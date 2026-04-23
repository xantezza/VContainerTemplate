using System.Collections.Generic;
using System.Text;
using Unity.Services.Analytics;
using UnityEngine;
using VContainer;
using VContainerTemplate.Infrastructure.Services.Log;
using VContainerTemplate.Infrastructure.StateMachines.InitializationStateMachine.States;

namespace VContainerTemplate.Infrastructure.Services.Analytics
{
    public class UnityAnalyticsService : IAnalyticsService
    {
        [Inject]
        public UnityAnalyticsService()
        {
            Application.focusChanged += OnApplicationFocus;
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus) return;
            if (!Application.isPlaying) return;
            if (!InitializeUnityServicesState.IsInitialized) return;

            AnalyticsService.Instance.Flush();
        }

        public void SendEvent(string eventName)
        {
            if (!InitializeUnityServicesState.IsInitialized) return;

            Log.Log.Trace($"{eventName} sent", LogTag.Analytics);
#if DEV
            return;
#endif
#pragma warning disable CS0162
            AnalyticsService.Instance.RecordEvent(eventName);
#pragma warning restore CS0162
        }

        public void SendEvent(string eventName, Dictionary<string, object> paramsDictionary)
        {
            if (!InitializeUnityServicesState.IsInitialized) return;

            var customEvent = new CustomEvent(eventName);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{eventName} sent with params:");

            foreach (var (key, value) in paramsDictionary)
            {
                stringBuilder.AppendLine($"\n{key}: {value}");
                customEvent.Add(key, value);
            }

            Log.Log.Trace(stringBuilder.ToString(), LogTag.Analytics);
#if DEV
            return;
#endif
#pragma warning disable CS0162
            AnalyticsService.Instance.RecordEvent(customEvent);
#pragma warning restore CS0162
        }

        public void SendEvent(string eventName, object data)
        {
            SendEvent(eventName, new Dictionary<string, object> {["data"] = data});
        }
    }
}