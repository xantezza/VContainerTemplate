using System;
using Cysharp.Threading.Tasks;
using TriInspector;
using UnityEngine;
using VContainer;

namespace VContainerTemplate.Infrastructure.Windows
{
    [DeclareBoxGroup("Buttons")]
    public abstract class WindowBase<T> : MonoBehaviour, IWindowBase where T : IWindowBase
    {
        public Action<Type> OnBeforeShow { get; set; }
        public Action<Type> OnBeforeHide { get; set; }
        public Action<Type> OnAfterShow { get; set; }
        public Action<Type> OnAfterHide { get; set; }

        [field: SerializeField, ReadOnly] public bool IsShowing { get; protected set; } = false;
        [field: SerializeField, ReadOnly] public bool InQueue { get; set; } = false;
        [field: SerializeField, ReadOnly] public int QueuePriority { get; protected set; } = 0;

        public IWindowsService WindowService { get; private set; }

        private bool _isTransitioning;

        [Inject]
        protected virtual void Inject(IWindowsService windowsService)
        {
            WindowService = windowsService;
        }

        protected virtual void Start()
        {
            WindowService.RegisterWindow(this);
            StartAction();
        }

        private void OnDestroy()
        {
            WindowService.UnregisterWindow(this);
            DestroyAction();
        }

        [Group("Buttons")]
        [Button("Show window")]
        public async UniTask Show()
        {
            if (_isTransitioning || IsShowing) return;

            _isTransitioning = true;
            try
            {
                OnBeforeShow?.Invoke(GetType());
                await ShowAction();
                IsShowing = true;
                OnAfterShow?.Invoke(GetType());
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        [Group("Buttons")]
        [Button("Hide window")]
        public async UniTask Hide()
        {
            if (_isTransitioning || !IsShowing) return;

            _isTransitioning = true;
            try
            {
                OnBeforeHide?.Invoke(GetType());
                await HideAction();
                IsShowing = false;
                OnAfterHide?.Invoke(GetType());
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        public UniTask Toggle() => IsShowing ? Hide() : Show();

        protected virtual void StartAction()
        {
            if (IsShowing || gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                IsShowing = false;
            }
        }

        protected virtual void DestroyAction()
        {
        }

        protected virtual UniTask ShowAction()
        {
            gameObject.SetActive(true);
            return default;
        }

        protected virtual UniTask HideAction()
        {
            gameObject.SetActive(false);
            return default;
        }
    }
}