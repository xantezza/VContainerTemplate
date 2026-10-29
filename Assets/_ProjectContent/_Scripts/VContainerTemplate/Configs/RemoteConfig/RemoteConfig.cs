using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;
using VContainerTemplate.Configs.RemoteConfig.Configs;
using VContainerTemplate.Infrastructure.Logger;

namespace VContainerTemplate.Configs.RemoteConfig
{
    public static class RemoteConfig
    {
        public static Subject<Unit> OnInitializeAny =  new Subject<Unit>();
        public static Subject<Unit> OnInitializeDefault = new Subject<Unit>();
        public static Subject<Unit> OnInitializeRemote = new Subject<Unit>();

        public static MainConfig Infrastructure { get; private set; }

        private static JToken _cachedDefaultConfig;
        private static JToken _remoteConfig;

        private static bool _hasInitializedByRemote;

        public static void InitializeByDefault(JToken cachedDefaultConfig)
        {
            _cachedDefaultConfig = cachedDefaultConfig;

            if (_hasInitializedByRemote) return;

            ParseConfigs();
            OnInitializeDefault?.OnNext(default);
            OnInitializeAny?.OnNext(default);
        }

        public static void InitializeByRemote(JToken remoteConfig)
        {
            _hasInitializedByRemote = true;
            _remoteConfig = remoteConfig;
            ParseConfigs();
            OnInitializeRemote?.OnNext(default);
            OnInitializeAny?.OnNext(default);
        }

        private static void ParseConfigs()
        {
            Infrastructure = Parse<MainConfig>(RemoteConfigType.MainConfig);
        }

        private static T Parse<T>(string type) where T : IConfig, new()
        {
            try
            {
                return InternalParse(_remoteConfig ?? _cachedDefaultConfig);
            }
            catch (Exception exception1)
            {
                try
                {
                    Log.Warn($"Failed to parse remote config \"{type}\", using cached default. Exception: {exception1}", LogTag.UnityServices);

                    return InternalParse(_cachedDefaultConfig);
                }
                catch (Exception exception2)
                {
                    Log.Warn($"Failed to parse default config \"{type}\", using scripted default. Exception: {exception2}", LogTag.UnityServices);
                    return new T();
                }
            }

            T InternalParse(JToken config)
            {
                var configString = config[type].ToString();

                Log.Trace($"Parsing config: \n{type}: \n{configString}", LogTag.UnityServices);

                return JsonConvert.DeserializeObject<T>(configString);
            }
        }
    }
}