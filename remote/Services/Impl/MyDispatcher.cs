using System;
using System.Windows.Threading;

namespace remote
{
    public class MyDispatcher : IDispatcher
    {
        private Dispatcher _dispatcher;

        public void SetDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void BeginInvoke(Action action)
        {
            if (_dispatcher != null)
                _dispatcher.BeginInvoke(action);
        }

        public void Invoke(Action action)
        {
            if (_dispatcher != null)
                _dispatcher.Invoke(action);
        }
    }
}