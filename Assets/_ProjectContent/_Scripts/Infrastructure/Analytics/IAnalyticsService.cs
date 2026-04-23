using System.Collections.Generic;

namespace VContainerTemplate.Infrastructure.Services.Analytics
{
    public interface IAnalyticsService
    {
        void SendEvent(string eventName);
        void SendEvent(string eventName, Dictionary<string, object> paramsDictionary);
        void SendEvent(string eventName, object data);
    }
}