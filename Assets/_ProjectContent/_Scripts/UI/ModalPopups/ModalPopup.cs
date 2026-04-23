using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainerTemplate.Infrastructure.Services.Log;
using VContainerTemplate.Utilities.Extensions;

namespace VContainerTemplate.UI.ModalPopups
{
    [Serializable]
    public class ModalPopupSettings
    {
        public Button CloseButton;
        public RectTransform ResizableRoot;
        public Image FadeImage;
        public float StartSize = 0.0f;
        public float MaxSize = 1.1f;
        public float ShowTime = 0.3f;
        public float HideTime = 0.3f;
    }

    public abstract class ModalPopup : MonoBehaviour
    {
        public readonly ReactiveCommand OnInteract = new();

        [SerializeField] private ModalPopupSettings _modalSettings;
        protected IModalPopupFactory _modalPopupFactory;

        public void SetModalPopupFactory(IModalPopupFactory modalPopupFactory)
        {
            _modalPopupFactory = modalPopupFactory;
        }

        public void Show()
        {
            if (_modalSettings.ResizableRoot != null)
            {
                _modalSettings.ResizableRoot.localScale = _modalSettings.StartSize * Vector3.one;
                gameObject.SetActive(true);
                _modalSettings.FadeImage.DOFade(0.7f, _modalSettings.ShowTime);
                _modalSettings.ResizableRoot.DOPopOutScale(
                    Vector3.one * _modalSettings.MaxSize,
                    Vector3.one,
                    _modalSettings.ShowTime / 2,
                    _modalSettings.ShowTime / 2
                );
            }

            if (_modalSettings.CloseButton != null) _modalSettings.CloseButton.onClick.AddListener(Hide);
        }

        private async void Hide()
        {
            try
            {
                if (_modalSettings.CloseButton != null) _modalSettings.CloseButton.onClick.RemoveListener(Hide);

                if (_modalSettings.ResizableRoot != null)
                {
                    _modalSettings.FadeImage.DOFade(0, _modalSettings.HideTime);
                    await _modalSettings.ResizableRoot.DOPopOutScale(
                        Vector3.one * _modalSettings.MaxSize,
                        Vector3.one * _modalSettings.StartSize,
                        _modalSettings.HideTime / 2,
                        _modalSettings.HideTime / 2
                    );
                }

                OnInteract.Execute();
                ReturnToPool();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        protected abstract void ReturnToPool();
    }
}