using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VContainerTemplate.Infrastructure.Services.Log;

namespace VContainerTemplate.Utilities.ScriptableObjects
{
    /// <summary>
    /// Auto Creates Asset in _ProjectContent/Data when you inherit it
    /// </summary>
    public abstract class ACScriptableObject : ScriptableObject
    {
        private const string AutoPath = "_ProjectContent/Data";

        private string GetAssetName()
        {
            return GetType().Name;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void AutoCreateAll()
        {
            var types = TypeCache.GetTypesDerivedFrom<ACScriptableObject>()
                .Where(t => !t.IsAbstract);

            foreach (var type in types)
            {
                var method = typeof(ACScriptableObject).GetMethod(nameof(EnsureAssetExists),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                var genericMethod = method.MakeGenericMethod(type);
                genericMethod.Invoke(null, null);
            }
        }

        private static void EnsureAssetExists<T>() where T : ACScriptableObject
        {
            var assetName = GetAssetNameFromType(typeof(T));
            CreateAssetInDataFolder<T>(assetName);
        }

        private static string GetAssetNameFromType(Type type)
        {
            var tempInstance = CreateInstance(type) as ACScriptableObject;
            var name = tempInstance.GetAssetName();
            DestroyImmediate(tempInstance);
            return name;
        }

        private static void CreateAssetInDataFolder<T>(string assetName) where T : ScriptableObject
        {
            var dataFolder = $"Assets/{AutoPath}";
            if (!AssetDatabase.IsValidFolder(dataFolder))
                AssetDatabase.CreateFolder("Assets", AutoPath);

            var assetPath = $"{dataFolder}/{assetName}.asset";

            var existingAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (existingAsset != null)
            {
                return;
            }

            var asset = CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Log.Trace($"Asset created: {assetPath}");
        }
#endif
    }
}