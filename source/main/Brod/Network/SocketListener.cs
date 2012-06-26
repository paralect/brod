using System;
using System.IO;
using System.Threading;
using Brod.Responses;
using Brod.Tasks.Abstract;

namespace Brod.Network
{
    public class SocketListener : ITask
    {
        private readonly ZMQ.SocketType _socketType;
        private readonly Func<byte[], Response> _handler;
        private readonly String _address;
        private ZMQ.Context _zeromqContext;

        public SocketListener(ZMQ.SocketType socketType, Int32 port, Func<byte[], Response> handler)
        {
            _socketType = socketType;
            _handler = handler;
            _address = String.Format("tcp://*:{0}", port);
        }

        public void Run(CancellationToken token)
        {
            using (Socket socket = CreateSocket(_socketType))
            {
                // Bind to socket
                socket.Bind(_address);

                // Process while canellation not requested
                while (!token.IsCancellationRequested)
                {
                    // Waits for messages
                    var data = socket.Recv();
                    if (data == null) continue;

                    var response = _handler(data);

                    if (response != null)
                    {
                        using (var responseStream = new MemoryStream())
                        using (var responseWriter = new BinaryWriter(responseStream))
                        {
                            response.WriteToStream(responseStream, responseWriter);
                            socket.Send(responseStream.ToArray());
                        }
                    }
                }
            }
        }

        public void Init()
        {
            _zeromqContext = new ZMQ.Context(1);
        }

        public void Dispose()
        {
            if (_zeromqContext != null)
                _zeromqContext.Dispose();
        }

        public Socket CreateSocket(ZMQ.SocketType socketType)
        {
            var zmqsocket = _zeromqContext.Socket(socketType);
            var socket = new Socket(zmqsocket);
            return socket;
        }
    }
}