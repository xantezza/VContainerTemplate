using TMPro;
using UnityEngine;

namespace VContainerTemplate.UI.Menu
{
    public class VersionDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text versionText;

        private void Start()
        {
            versionText.SetText(Application.version);
        }
    }
}