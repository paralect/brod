using System;
using System.Globalization;
using System.Threading;
using Brod.Common;
using Brod.Contracts.Requests;
using Brod.Contracts.Responses;

namespace Brod.Network
{
    public class RequestSender : IDisposable
    {
        private readonly ZMQ.Context _context;
        private readonly String _address;
        private readonly Socket _socket;

        public RequestSender(String address, ZMQ.SocketType socketType, ZMQ.Context context)
        {
            _context = context;
            _address = Protocolize(address);
            _socket = CreateSocket(socketType);
            _socket.Connect(_address, CancellationToken.None);
        }

        public Response Send(Request request)
        {
            return Send(request, true);
        }

        public void Push(Request request)
        {
            Send(request, false);
        }

        private Response Send(Request request, bool responseAvailable)
        {
            using (var memoryStream = new BinaryMemoryStream())
            {
                request.WriteToStream(memoryStream);
                var data = memoryStream.ToArray();
                _socket.Send(data);
            }

            if (!responseAvailable)
                return null;

            var result = _socket.Recv();

            using (var stream = new BinaryMemoryStream(result))
            {
                var responseType = (ResponseType) stream.Reader.ReadInt16();

                switch (responseType)
                {
                    case ResponseType.BrokerInfoResponse:
                        return BrokerInfoResponse.ReadFromStream(stream);
                    case ResponseType.FetchResponse:
                        return FetchResponse.ReadFromStream(stream);
                }

                return null;
            }
        }

        /// <summary>
        /// Insures that protocol is specified. If it doesn't - use tcp://
        /// </summary>
        private String Protocolize(String address)
        {
            if (address.StartsWith("tcp://", false, CultureInfo.InvariantCulture))
                return address;

            return "tcp://" + address;
        }

        private Socket CreateSocket(ZMQ.SocketType socketType)
        {
            var zmqsocket = _context.Socket(socketType);
            var socket = new Socket(zmqsocket);
            return socket;
        }

        public void Dispose()
        {
            if (_socket != null)
                _socket.Dispose();
        }
    }
}