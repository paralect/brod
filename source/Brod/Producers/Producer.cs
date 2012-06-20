using System;
using System.IO;
using System.Threading;
using Brod.Sockets;

namespace Brod.Producers
{
    public class Producer : IDisposable
    {
        private readonly string _address;
        private readonly ZMQ.Context _zeromqContext;
        private readonly Socket _pushSocket;

        public Producer(String address, ZMQ.Context zeromqContext)
        {
            _address = address;
            _zeromqContext = zeromqContext;
            _pushSocket = CreateSocket(ZMQ.SocketType.PUSH);

            // Bind to socket
            _pushSocket.Connect(_address, CancellationToken.None);                
        }

        public void Send(byte[] payload)
        {
            using(var stream = new MemoryStream())
            using(var writer = new MessageWriter(stream))
            {
                writer.WriteMessage(payload);

                var data = stream.ToArray();

                Console.WriteLine("Sending {0} bytes", data.Length);
                _pushSocket.Send(data);
            }
        }

        public Socket CreateSocket(ZMQ.SocketType socketType)
        {
            var zmqsocket = _zeromqContext.Socket(socketType);
            var socket = new Socket(zmqsocket);
            return socket;
        }

        public void Dispose()
        {
            if (_pushSocket != null)
                _pushSocket.Dispose();
        }
    }
}