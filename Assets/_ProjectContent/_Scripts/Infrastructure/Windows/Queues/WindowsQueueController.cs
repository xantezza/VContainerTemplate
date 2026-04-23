using System;
using VContainerTemplate.Infrastructure.Services.Log;

namespace VContainerTemplate.Infrastructure.Services.Windows.Queues
{
    public class WindowsQueueController
    {
        private readonly IWindowsService _windowsService;
        private WindowsQueue _mainQueue;

        public WindowsQueueController(IWindowsService windowsService)
        {
            _windowsService = windowsService;
            _mainQueue = new WindowsQueue(windowsService);
        }

        public void Next() => _mainQueue?.Next();

        public void RunQueue()
        {
            if (_mainQueue == null)
                _mainQueue = new WindowsQueue(_windowsService);
            if (_mainQueue.IsRunning)
                return;

            _mainQueue.OnQueueFinished += OnQueueFinished;

            if (_mainQueue.Run())
            {
                Log.Log.Trace($"Windows queue is run!", LogTag.WindowsQueueController);
            }
            else
            {
                Log.Log.Error($"Windows queue run is aborted!", LogTag.WindowsQueueController);
            }
        }

        public void AddWindowsGroupInQueue(WindowsGroupBase windowsGroupBase)
        {
            foreach (Type windowType in windowsGroupBase.WindowTypes)
                AddWindowByTypeInQueue(windowType);
        }

        public void AddWindowByTypeInQueue(Type type)
        {
            if (_mainQueue.AddWindowByTypeIn(type))
            {
                Log.Log.Trace($"A new window '{type.ToString().Split('.')[^1]}' has been successfully added to the window queue.", LogTag.WindowsQueueController);
            }
            else
            {
                Log.Log.Error($"Couldn't add window '{type.ToString().Split('.')[^1]}' was not added to the queue because the window does not exist in the context of the scene.", LogTag.WindowsQueueController);
            }
        }

        public void AddWindowInQueue(IWindowBase window)
        {
            if (_mainQueue.AddWindowIn(window))
            {
                Log.Log.Trace($"A new window '{window.GetType().ToString().Split('.')[^1]}' has been successfully added to the window queue.", LogTag.WindowsQueueController);
            }
            else
            {
                Log.Log.Warn($"Couldn't add window '{window.GetType().ToString().Split('.')[^1]}' was not added to the queue because it already exists.", LogTag.WindowsQueueController);
            }
        }

        private void OnQueueFinished()
        {
            _mainQueue.OnQueueFinished -= OnQueueFinished;
#if DEV
            Log.Log.Trace($"Windows queue is finished!", LogTag.WindowsQueueController);
#endif
        }
    }
}