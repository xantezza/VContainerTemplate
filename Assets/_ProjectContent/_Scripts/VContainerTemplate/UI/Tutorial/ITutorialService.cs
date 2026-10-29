using UnityEngine.UI;

namespace VContainerTemplate.UI.Tutorial
{
    public interface ITutorialService
    {
        void LockToButton(Button button, bool animated = true, bool autoHideOnClick = true);
        void Hide();
    }
}