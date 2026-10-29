using System;
using Newtonsoft.Json;

namespace VContainerTemplate.Configs.RemoteConfig.Configs
{
    [Serializable]
    public class MainConfig : IConfig
    {
        [JsonProperty] public float FakeMinimalLoadTime = 0.1f;
    }
}