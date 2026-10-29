using UnityEditor;
using UnityEngine;

namespace VContainerTemplateEditor.Utils.Static
{
    public static class MenuItems
    {
        [MenuItem("Assets/Project/Select Config Utility", false, 8)]
        private static void SelectConfigUtility()
        {
            Selection.activeObject = Resources.Load<ConfigUtility>("ConfigUtility");
        }

        [MenuItem("Assets/Project/Open PersistentDataPath", false, 10)]
        private static void OpenPersistentDataPath()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

    }
}