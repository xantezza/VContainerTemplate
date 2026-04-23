using Cysharp.Threading.Tasks;

namespace VContainerTemplate.Infrastructure.Services.LoadingCurtain
{
    public class LoadingCurtainPresenter : ILoadingCurtainPresenter
    {
        private readonly LoadingCurtainView _view;

        public LoadingCurtainPresenter(LoadingCurtainView view)
        {
            _view = view;
        }
        
        public async UniTask Show(float tweenDuration = 0.3f)
        {
            await _view.Show(tweenDuration);
        }

        public void ForceShow()
        {
            _view.ForceShow();
        }

        public void SetProgress01(float value)
        {
            _view.SetProgress01(value);
        }

        public async UniTask Hide(float tweenDuration = 0.3f)
        {
            await _view.Hide(tweenDuration);
        }

        public void ForceHide()
        {
            _view.ForceHide();
        }
    }
}