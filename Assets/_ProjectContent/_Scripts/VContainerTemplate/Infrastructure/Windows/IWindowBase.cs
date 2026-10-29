using System;
using Cysharp.Threading.Tasks;

namespace VContainerTemplate.Infrastructure.Windows
{
    public interface IWindowBase
    {
        public Action<Type> OnBeforeShow { get; set; }
        public Action<Type> OnBeforeHide { get; set; }
        public Action<Type> OnAfterShow { get; set; }
        public Action<Type> OnAfterHide { get; set; }
        public UniTask Show();
        public UniTask Hide();
        public UniTask Toggle();
        public bool IsShowing { get; }
        public bool InQueue { get; set; }
        public IWindowsService WindowService { get; }
        public int QueuePriority { get; }
    }
}