using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VContainerTemplate.Infrastructure.Windows.Queues;

namespace VContainerTemplate.Infrastructure.Windows
{
    public interface IWindowsService
    {
        public WindowsQueueController QueueController { get; }
        public HashSet<Type> ShowedWindows { get; }
        public UniTask ShowWindow(Type type);
        public UniTask HideWindow(Type type);
        public UniTask ToggleWindow(Type type);
        public UniTask ShowWindow<T>() where T : IWindowBase;
        public UniTask HideWindow<T>() where T : IWindowBase;
        public UniTask ToggleWindow<T>() where T : IWindowBase;
        public void RegisterWindow<T>(WindowBase<T> windowType) where T : IWindowBase;
        public void UnregisterWindow<T>(WindowBase<T> windowType) where T : IWindowBase;
        public TWindow GetWindow<TWindow>() where TWindow : class, IWindowBase;
        public IWindowBase GetWindow(Type type);
        public bool TryGetWindow<TWindow>(out TWindow window) where TWindow : class, IWindowBase;
        public bool TryGetWindow(Type type, out IWindowBase window);
        public bool ExistWindow(Type type);
    }
}