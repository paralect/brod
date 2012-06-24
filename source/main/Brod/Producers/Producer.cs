using System;
using System.IO;
using System.Text;
using System.Threading;
using Brod.Requests;
using Brod.Sockets;

namespace Brod.Producers
{
    public class Producer
    {
        private readonly string _address;
        private readonly ZMQ.Context _zeromqContext;

        public Producer(String address, ZMQ.Context zeromqContext)
        {
            _address = address;
            _zeromqContext = zeromqContext;
        }

        public ProducerMessageStream OpenMessageStream(String topic)
        {
            return new ProducerMessageStream(_address, topic, _zeromqContext);
        }
    }
}