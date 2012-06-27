using System;
using System.IO;
using System.Threading;
using Brod.Contracts.Requests;
using Brod.Contracts.Responses;
using Brod.Tasks.Abstract;

namespace Brod.Network
{
    public class SocketListener : ITask
    {
        private readonly ZMQ.SocketType _socketType;
        private readonly Func<RequestType, Stream, BinaryReader, Func<Stream, BinaryReader, Response>> _handlerMapping;
        private readonly String _address;
        private ZMQ.Context _zeromqContext;

        public SocketListener(ZMQ.SocketType socketType, Int32 port,
            Func<RequestType, Stream, BinaryReader, Func<Stream, BinaryReader, Response>> handlerMapping)
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

                    using (var requestStream = new MemoryStream(data))
                    using (var requestReader = new BinaryReader(requestStream))
                    {
                        // by request type we can distinguish actual request
                        var requestType = (RequestType) requestReader.ReadInt16();
                        var handler = _handlerMapping(requestType, requestStream, requestReader);

                        response = handler(requestStream, requestReader);
                    }

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