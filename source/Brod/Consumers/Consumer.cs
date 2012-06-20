using System;
using System.IO;
using System.Threading;
using Brod.Sockets;

namespace Brod.Consumers
{
    public class Consumer : IDisposable
    {
        private readonly string _address;
        private readonly ZMQ.Context _zeromqContext;
        private readonly Socket _reqSocket;

        public Consumer(String address, ZMQ.Context zeromqContext)
        {
            _address = address;
            _zeromqContext = zeromqContext;
            _reqSocket = CreateSocket(ZMQ.SocketType.REQ);

            // Bind to socket
            _reqSocket.Connect(_address, CancellationToken.None);                
        }

        public byte[] Get(byte[] payload)
        {
            _reqSocket.Send(payload);
            return _reqSocket.Recv();
        }

        public Socket CreateSocket(ZMQ.SocketType socketType)
        {
            var zmqsocket = _zeromqContext.Socket(socketType);
            var socket = new Socket(zmqsocket);
            return socket;
        }

        public void Dispose()
        {
            if (_reqSocket != null)
                _reqSocket.Dispose();
        }
    }
}