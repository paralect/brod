using System;
using System.IO;
using System.Threading;
using Brod.Sockets;

namespace Brod.Producers
{
    public class Producer
    {
        private readonly string _address;
        private ZMQ.Context _zeromqContext;

        public Producer(String address)
        {
            _address = address;
            _zeromqContext = new ZMQ.Context(2);
        }

        public void Send(byte[] payload)
        {
            using (Socket pushSocket = CreateSocket(ZMQ.SocketType.PUSH))
            {
                // Bind to socket
                pushSocket.Connect(_address, CancellationToken.None);

                using(var stream = new MemoryStream())
                using(var writer = new MessageWriter(stream))
                {
                    writer.WriteMessage(payload);
                    pushSocket.Send(stream.GetBuffer());
                }
            }
        }

        public Socket CreateSocket(ZMQ.SocketType socketType)
        {
            var zmqsocket = _zeromqContext.Socket(socketType);
            var socket = new Socket(zmqsocket);
            return socket;
        }
    }
}