using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using VContainerTemplate.Infrastructure.Logger;

namespace VContainerTemplate.Infrastructure.Windows.Queues
{
    public class WindowsQueue
    {
        public Action OnQueueFinished { get; set; }
        public Action OnQueueRunned { get; set; }
        public bool IsRunning { get; private set; }

        private readonly IWindowsService _windowsService;
        private readonly Dictionary<int, Queue<IWindowBase>> _queueItems = new();
        private int _currentQueueIndex;

        private bool _interruptRequested;
        private UniTaskCompletionSource _interruptSignal = new();
        private bool _stopRequested;

        public WindowsQueue(IWindowsService windowsService)
        {
            _windowsService = windowsService;
        }

        public bool AddWindowByTypeIn(Type type)
        {
            if (_windowsService.TryGetWindow(type, out var window))
                return AddWindowIn(window);
            return false;
        }

        public bool AddWindowIn(IWindowBase window)
        {
            ValidatePriority(window.QueuePriority);
            if (ExistWindowInQueue(window))
                return false;

            window.InQueue = true;
            _queueItems[window.QueuePriority].Enqueue(window);

            if (IsRunning)
            {
                if (window.QueuePriority > _currentQueueIndex)
                    RequestInterrupt();
            }
            else
            {
                _windowsService.QueueController.RunQueue();
            }

            return true;
        }

        public bool AddWindowIn<T>(WindowBase<T> window) where T : class, IWindowBase => AddWindowIn((IWindowBase) window);

        public bool ExistWindowInQueue(IWindowBase window)
        {
            return _queueItems[window.QueuePriority].Contains(window);
        }

        public void ValidatePriority(int priority)
        {
            if (!_queueItems.ContainsKey(priority))
                _queueItems.Add(priority, new Queue<IWindowBase>());
        }

        public bool Run()
        {
            if (IsRunning) return false;

            IsRunning = true;
            _stopRequested = false;
            _interruptRequested = false;
            _interruptSignal = new UniTaskCompletionSource();
            OnQueueRunned?.Invoke();

            RunInternal().Forget(OnQueueError);

            return true;
        }

        public void Stop()
        {
            if (!IsRunning) return;

            _stopRequested = true;
            RequestInterrupt();
        }

        private async UniTask RunInternal()
        {
            try
            {
                while (!_stopRequested && _queueItems.Count > 0)
                {
                    _currentQueueIndex = _queueItems.Keys.Max();
                    var windows = _queueItems[_currentQueueIndex];

                    var window = windows.Peek();
                    window.InQueue = false;

                    await window.Show();

                    if (await WaitUntilHidden(window))
                    {
                        windows.Dequeue();
                        if (windows.Count == 0)
                            _queueItems.Remove(_currentQueueIndex);
                    }
                }
            }
            finally
            {
                Clear();
                OnQueueFinished?.Invoke();
            }
        }

        private async UniTask<bool> WaitUntilHidden(IWindowBase window)
        {
            var hidden = new UniTaskCompletionSource();
            Action<Type> onHidden = _ => hidden.TrySetResult();
            window.OnAfterHide += onHidden;

            try
            {
                while (!hidden.Task.GetAwaiter().IsCompleted)
                {
                    if (_stopRequested)
                    {
                        await window.Hide();
                        return false;
                    }

                    if (HasHigherPriorityQueued)
                    {
                        await window.Hide();
                        return false;
                    }

                    if (_interruptRequested)
                    {
                        _interruptRequested = false;
                        _interruptSignal = new UniTaskCompletionSource();
                        continue;
                    }

                    var winner = await UniTask.WhenAny(hidden.Task, _interruptSignal.Task);
                    if (winner == 0) return true;
                }

                return true;
            }
            finally
            {
                window.OnAfterHide -= onHidden;
            }
        }

        private bool HasHigherPriorityQueued =>
            _queueItems.Count > 0 && _queueItems.Keys.Max() > _currentQueueIndex;

        private void RequestInterrupt()
        {
            _interruptRequested = true;
            _interruptSignal.TrySetResult();
        }

        private void Clear()
        {
            _currentQueueIndex = 0;
            _queueItems.Clear();
            IsRunning = false;
        }

        private void OnQueueError(Exception exception)
        {
            Log.Error($"WindowsQueue failed unexpectedly:\n{exception}", LogTag.WindowsQueueController);
        }
    }
}