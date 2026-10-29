using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainerTemplate.Infrastructure.Logger;
using VContainerTemplate.Infrastructure.Windows.Queues;

namespace VContainerTemplate.Infrastructure.Windows
{
    public class WindowsService : IWindowsService
    {
        private readonly Dictionary<Type, IWindowBase> _windows = new();
        public HashSet<Type> ShowedWindows { get; } = new();
        public WindowsQueueController QueueController { get; }

        [Inject]
        public WindowsService()
        {
            QueueController = new WindowsQueueController(this);
        }

        public bool ExistWindow(Type type) =>
            _windows.ContainsKey(type);

        public TWindow GetWindow<TWindow>() where TWindow : class, IWindowBase
        {
            if (_windows.TryGetValue(typeof(TWindow), out var window))
                return window as TWindow;
            return null;
        }

        public IWindowBase GetWindow(Type type) =>
            _windows.GetValueOrDefault(type);

        public bool TryGetWindow<TWindow>(out TWindow window) where TWindow : class, IWindowBase
        {
            if (_windows.TryGetValue(typeof(TWindow), out var value))
            {
                window = value as TWindow;
                return true;
            }

            window = null;
            return false;
        }

        public bool TryGetWindow(Type type, out IWindowBase window) =>
            _windows.TryGetValue(type, out window);

        public async UniTask ShowWindow(Type type)
        {
            if (_windows.TryGetValue(type, out var window))
                await window.Show();
        }

        public async UniTask HideWindow(Type type)
        {
            if (_windows.TryGetValue(type, out var window))
                await window.Hide();
        }

        public async UniTask ToggleWindow(Type type)
        {
            if (_windows.TryGetValue(type, out var window))
                await window.Toggle();
        }

        public UniTask ShowWindow<T>() where T : IWindowBase => ShowWindow(typeof(T));

        public UniTask HideWindow<T>() where T : IWindowBase => HideWindow(typeof(T));
        public UniTask ToggleWindow<T>() where T : IWindowBase => ToggleWindow(typeof(T));

        public void RegisterWindow<T>(WindowBase<T> window) where T : IWindowBase
        {
            _windows.Add(typeof(T), window);
            window.OnAfterHide += OnAfterWindowHide;
            window.OnAfterShow += OnAfterShow;
            Log.Trace($"Registering window of type {typeof(T).ToString().Split('.')[^1]}", LogTag.WindowService);
        }

        public void UnregisterWindow<T>(WindowBase<T> window) where T : IWindowBase
        {
            _windows.Remove(typeof(T));
            window.OnAfterHide -= OnAfterWindowHide;
            window.OnAfterShow -= OnAfterShow;
            Log.Trace($"Unregistering window of type {typeof(T).ToString().Split('.')[^1]}", LogTag.WindowService);
        }

        private void OnAfterShow(Type windowType) =>
            ShowedWindows.Add(windowType);

        private void OnAfterWindowHide(Type windowType) =>
            ShowedWindows.Remove(windowType);
    }
}