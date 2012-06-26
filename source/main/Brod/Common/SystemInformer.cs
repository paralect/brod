using System;
using System.Diagnostics;

namespace Brod.Common
{
    public class SystemInformer
    {
        /// <summary>
        /// All observers4
        /// </summary>
        private static IObserver<ISystemEvent>[] _observers = new IObserver<ISystemEvent>[0];

        /// <summary>
        /// Swap existing observers collection with a new one
        /// </summary>
        public static IObserver<ISystemEvent>[] Swap(params IObserver<ISystemEvent>[] swap)
        {
            var old = _observers;
            _observers = swap;
            return old;
        }

        /// <summary>
        /// Notify system about some event
        /// </summary>
        public static void Notify(ISystemEvent @event)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnNext(@event);
                }
                catch (Exception ex)
                {
                    var message = string.Format("Observer {0} failed with {1}", observer, ex);
                    Trace.WriteLine(message);
                }
            }
        }

        /// <summary>
        /// Inform all observers that no more notifications planned. 
        /// Possibly for shutdown phase.
        /// </summary>
        public static void Complete()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
        }
    }
}
