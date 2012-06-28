
using System;

namespace Brod.Producers
{
    /// <summary>
    /// ProducerContext is thread safe and may be shared among as many application threads as necessary, 
    /// without any additional locking required on the part of the caller.
    /// </summary>
    public class ProducerContext : IDisposable
    {
        /// <summary>
        /// ZMQ context that will be shared among many producers
        /// </summary>
        private readonly ZMQ.Context _zmqContext;

        /// <summary>
        /// ZMQ context that will be shared among many producers
        /// </summary>
        public ZMQ.Context ZmqContext
        {
            get { return _zmqContext; }
        }

        /// <summary>
        /// Creates ProducerContext
        /// </summary>
        public ProducerContext()
        {
            _zmqContext = new ZMQ.Context(1);
        }

        public void Dispose()
        {
            if (_zmqContext != null)
                _zmqContext.Dispose();
        }
    }
}