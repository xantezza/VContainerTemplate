using System;
using System.Collections.Generic;
using System.Text;
using Unity.Services.Analytics;
using UnityEngine;
using VContainer;
using VContainerTemplate.Infrastructure.Logger;
using VContainerTemplate.Infrastructure.StateMachines.Initialization.States;

namespace VContainerTemplate.Infrastructure.Analytics
{
    public class UnityAnalyticsService : IAnalyticsService, IDisposable
    {
        [Inject]
        public UnityAnalyticsService()
        {
            Application.logMessageReceived += OnApplicationLogMessageReceived;
#if !DEV
            Application.focusChanged += OnApplicationFocus;
#endif
        }

        public void Dispose()
        {
            Application.logMessageReceived -= OnApplicationLogMessageReceived;
            Application.focusChanged -= OnApplicationFocus;
        }

        private void OnApplicationLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (type is LogType.Exception or LogType.Error)
            {
                SendAnalyticsErrorEvent($"{condition}\n{stacktrace}");
            }
        }

        private void SendAnalyticsErrorEvent(string text)
        {
            SendEvent(AnalyticsNames.Exception, text);
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

            Log.Trace($"{eventName} sent", LogTag.Analytics);
#if !DEV
            AnalyticsService.Instance.RecordEvent(eventName);
#endif
        }

        public void SendEvent(string eventName, Dictionary<string, object> paramsDictionary)
        {
#if !DEV
            if (!InitializeUnityServicesState.IsInitialized) return;
#endif
            var customEvent = new CustomEvent(eventName);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{eventName} sent with params:");

            foreach (var (key, value) in paramsDictionary)
            {
                stringBuilder.AppendLine($"\n{key}: {value}");
                customEvent.Add(key, value);
            }

            Log.Trace(stringBuilder.ToString(), LogTag.Analytics);
#if !DEV
            AnalyticsService.Instance.RecordEvent(customEvent);
#endif
        }

        public void SendEvent(string eventName, object data)
        {
            SendEvent(eventName, new Dictionary<string, object> {["data"] = data});
        }
    }
}