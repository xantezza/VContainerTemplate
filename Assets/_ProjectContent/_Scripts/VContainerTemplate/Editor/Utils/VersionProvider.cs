using UnityEngine;
using VContainerTemplate.Utilities.ScriptableObjects;

namespace VContainerTemplateEditor.Utils
{
    public class VersionProvider : ACScriptableObject
    {
        [field: SerializeField] public int Major { get; private set; }
        public int Minor;
    }
}