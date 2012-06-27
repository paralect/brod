using System;
using System.IO;
using System.Threading;
using Brod.Common;
using Brod.Common.Tasks;
using Brod.Contracts.Requests;
using Brod.Contracts.Responses;

namespace Brod.Network
{
    public class SocketListener : ITask
    {
        private readonly ZMQ.SocketType _socketType;
        private readonly Func<RequestType, BinaryStream, Func<BinaryStream, Response>> _handlerMapping;
        private readonly String _address;
        private ZMQ.Context _zeromqContext;

        public SocketListener(ZMQ.SocketType socketType, Int32 port,
            Func<RequestType, BinaryStream, Func<BinaryStream, Response>> handlerMapping)
        {
            _socketType = socketType;
            _handlerMapping = handlerMapping;
            _address = String.Format("tcp://*:{0}", port);
        }

        public void Run(CancellationToken token)
        {
            using (Socket socket = CreateSocket(_socketType))
            {
                // Bind to address
                socket.Bind(_address);

                // Process while canellation not requested
                while (!token.IsCancellationRequested)
                {
                    // Waits for messages
                    var data = socket.Recv();
                    if (data == null) continue;

                    Response response = null;

                    using (var buffer = new BinaryMemoryStream(data))
                    {
                        // by request type we can distinguish actual request
                        var requestType = (RequestType)buffer.Reader.ReadInt16();
                        var handler = _handlerMapping(requestType, buffer);

                        response = handler(buffer);
                    }

                    if (response != null)
                    {
                        using (var buffer = new BinaryMemoryStream())
                        {
                            response.WriteToStream(buffer);
                            socket.Send(buffer.ToArray());
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