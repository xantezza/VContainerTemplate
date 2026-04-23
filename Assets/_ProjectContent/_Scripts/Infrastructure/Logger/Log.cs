using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using VContainerTemplate.Infrastructure.Services.Analytics;
using VContainerTemplate.Utilities.Extensions;
using Debug = UnityEngine.Debug;

namespace VContainerTemplate.Infrastructure.Services.Log
{
    public static class Log
    {
        private static IAnalyticsService _analyticsService;

        private static readonly LogTag[] _tagsToExclude = { };

        private static readonly Dictionary<LogTag, Color> _tagColors = new()
        {
            {LogTag.InitializationStateMachine, new Color(0.0f, 1, 0)},
            {LogTag.GameLoopStateMachine, new Color(0.3f, 0.99f, 0.7f)},
            {LogTag.SaveService, new Color(0.4f, 1f, 1f)},
            {LogTag.SceneLoader, new Color(0.7f, 0.5f, 1f)},
            {LogTag.Analytics, new Color(0.0f, 1f, 1f)},
            {LogTag.UnityServices, new Color(0.0f, 0.8f, 1f)}
        };

        private static bool IsColoredLogs => true;

        
        [Conditional("DEV"), Conditional("UNITY_EDITOR")]
        public static void Trace(string text, LogTag tag = LogTag.Default)
        {
            if (_tagsToExclude.Contains(tag)) return;

            if (_tagColors.TryGetValue(tag, out Color color) && IsColoredLogs)
            {
                Debug.Log($"{color.ColorizeWithBrackets(tag)} {text}");
            }
            else
            {
                Debug.LogFormat("[{0}] {1}", tag, text);
            }
        }

        [Conditional("DEV"), Conditional("UNITY_EDITOR")]
        public static void Warn(string text, LogTag tag = LogTag.Default)
        {
            if (_tagColors.TryGetValue(tag, out Color color) && IsColoredLogs)
            {
                Debug.LogWarning($"[Warn]{color.ColorizeWithBrackets(tag)} {text}");
            }
            else
            {
                Debug.LogWarningFormat("[Warn][{0}] {1}", tag, text);
            }
        }

        public static void Error(string text, LogTag tag = LogTag.Default)
        {
            if (_tagColors.TryGetValue(tag, out Color color) && IsColoredLogs)
            {
                Debug.LogError($"[Exception]{color.ColorizeWithBrackets(tag)} {text}");
            }
            else
            {
                Debug.LogErrorFormat("[Exception][{0}] {1}", tag, text);
            }
        }

        private static void OnApplicationLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (type is LogType.Exception or LogType.Error)
            {
                SendAnalyticsErrorEvent($"{condition}\n{stacktrace}");
            }
        }

        private static void SendAnalyticsErrorEvent(string text)
        {
            //_analyticsService.SendEvent(AnalyticsNames.Exception, text);
        }
    }
}