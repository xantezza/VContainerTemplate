using System;
using System.Collections.Generic;

namespace VContainerTemplate.Infrastructure.Windows.Queues
{
    public abstract class WindowsGroupBase
    {
        public List<Type> WindowTypes { get; private set; } = new List<Type>();

        protected void AddType<TWindow>() where TWindow : class, IWindowBase
        {
            WindowTypes.Add(typeof(TWindow));
        }
    }
}