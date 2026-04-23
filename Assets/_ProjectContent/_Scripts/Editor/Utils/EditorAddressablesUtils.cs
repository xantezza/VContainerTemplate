#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace VContainerTemplateEditor.Editor.Utils
{
    public class EditorAddressablesUtils
    {
        public static void SetIncludeGroupInBuild(string groupName, bool state)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.FindGroup(groupName);

            if (group != null)
            {
                var schema = group.GetSchema<BundledAssetGroupSchema>();
                if (schema != null)
                {
                    schema.IncludeInBuild = state;
                    EditorUtility.SetDirty(group);
                    AssetDatabase.SaveAssets();
                }
            }
        }
    }
}

#endif