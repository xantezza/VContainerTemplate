using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VContainerTemplate.Infrastructure.LoadingCurtain
{
    public class LoadingCurtainView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Slider _slider;
        private Tween _tween;

        private void Awake()
        {
            ForceHide();
        }

        public void ForceShow()
        {
            _tween?.Kill();
            SetProgress01(0);
            _canvasGroup.alpha = 1f;
            _canvas.enabled = true;
        }

        public void ForceHide()
        {
            _tween?.Kill();
            SetProgress01(1);
            _canvasGroup.alpha = 0f;
            _canvas.enabled = false;
        }

        public async UniTask Show(float tweenDuration = 0.3f)
        {
            _tween?.Kill();
            if (_canvas.enabled) return;
            SetProgress01(0);
            _canvas.enabled = true;
            _tween = _canvasGroup.DOFade(1f, tweenDuration);
            await _tween.ToUniTask();
        }

        public async UniTask Hide(float tweenDuration = 0.3f)
        {
            _tween?.Kill();
            if (!_canvas.enabled) return;
            SetProgress01(1);
            _tween = _canvasGroup.DOFade(0f, tweenDuration);
            await _tween.ToUniTask();
            _canvas.enabled = false;
        }

        public void SetProgress01(float value)
        {
            _slider.value = value;
        }
    }
}