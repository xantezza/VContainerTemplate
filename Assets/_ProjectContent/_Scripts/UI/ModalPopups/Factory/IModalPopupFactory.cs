using Cysharp.Threading.Tasks;

namespace VContainerTemplate.UI.ModalPopups
{
    public interface IModalPopupFactory
    {
        UniTask<T> Show<T>() where T : ModalPopup;
        UniTask<T> Spawn<T>() where T : ModalPopup;
        void ReturnToPool<T>(T modalPopup) where T : ModalPopup;
    }
}