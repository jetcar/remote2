using System;
using System.Windows.Threading;

namespace remote
{
    public interface IDispatcher
    {
        void Invoke(Action action);
        void SetDispatcher(Dispatcher dispatcher);
        void BeginInvoke(Action action);
    }
}