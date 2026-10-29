using System;
using Newtonsoft.Json;
using TriInspector;
using UnityEngine;
using VContainerTemplate.Configs.RemoteConfig;
using VContainerTemplate.Configs.RemoteConfig.Configs;
using VContainerTemplate.Utilities.ScriptableObjects;

namespace VContainerTemplateEditor.Utils
{
    public class ConfigUtility : ACScriptableObject
    {
        private enum Type
        {
            Main
        }

        [SerializeField] private Type type;


        [ShowIf(nameof(isMain)), SerializeField] private MainConfig _mainConfig;
        private bool isMain => type == Type.Main;
        
        #pragma warning disable 
        [SerializeField] private string _key;
        #pragma warning disable 
        
        [SerializeField] [TextArea(12, 9999)] private string _json;

        [Button] [PropertyOrder(-10)]
        private void ToJson()
        {
            switch (type)
            {
                case Type.Main:
                    _key = RemoteConfigType.MainConfig;
                    _json = JsonConvert.SerializeObject(_mainConfig, Formatting.Indented);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        [Button] [PropertyOrder(-10)]
        private void FromJson()
        {
            switch (type)
            {
                case Type.Main:
                    _mainConfig = JsonConvert.DeserializeObject<MainConfig>(_json);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}