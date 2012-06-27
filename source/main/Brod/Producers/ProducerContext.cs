using ZMQ;

namespace Brod.Producers
{
    /// <summary>
    /// ProducerContext is thread safe and may be shared among as many application threads as necessary, 
    /// without any additional locking required on the part of the caller.
    /// </summary>
    public class ProducerContext
    {
        private readonly ZMQ.Context _zeromqContext;

        public Context ZeromqContext
        {
            get { return _zeromqContext; }
        }

        public ProducerContext()
        {
            _zeromqContext = new ZMQ.Context(1);
        }

        public Producer CreateProducer(string brokerAddress)
        {
            return new Producer("tcp://" + brokerAddress, _zeromqContext);
        }
    }
}