using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Brod.Producers
{
    public class Producer : IDisposable
    {
        private readonly static ProducerContext _context = new ProducerContext();

        private readonly string _address;
        private readonly ZMQ.Context _zeromqContext;

        public Producer(String address)
        {
            _address = "tcp://" + address;
            _zeromqContext = _context.ZeromqContext;
        }

        public Producer(String address, ZMQ.Context zeromqContext)
        {
            _address = address;
            _zeromqContext = zeromqContext;
        }

        public ProducerMessageStream OpenStream(String topic)
        {
            return new ProducerMessageStream(_address, topic, _zeromqContext);
        }

        public void Dispose()
        {
            
        }
    }
}