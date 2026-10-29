using UnityEngine;
using UnityEngine.SceneManagement;
using VContainerTemplate.Infrastructure;

namespace VContainerTemplate.Utilities.MonoBehaviours
{
    public class EntryPointLoader : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Awake()
        {
            if (!EntryPoint.HasStarted) SceneManager.LoadScene(0);
        }
#endif
    }
}