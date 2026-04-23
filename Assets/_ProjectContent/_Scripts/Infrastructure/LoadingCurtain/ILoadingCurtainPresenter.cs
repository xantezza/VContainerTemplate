using Cysharp.Threading.Tasks;

namespace VContainerTemplate.Infrastructure.Services.LoadingCurtain
{
    public interface ILoadingCurtainPresenter
    {
        UniTask Show(float tweenDuration = 0.3f);
        void ForceShow();
        void SetProgress01(float value);
        UniTask Hide(float tweenDuration = 0.3f);
        void ForceHide();
    }
}