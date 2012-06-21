using System;
using System.IO;
using System.Text;
using System.Threading;
using Brod.Requests;
using Brod.Responses;
using Brod.Sockets;

namespace Brod.Tasks
{
    public class HistoryHandlerTask : ITask
    {
        private readonly BrokerConfiguration _configuration;
        private readonly Storage _storage;
        private readonly string _repAddress;
        private ZMQ.Context _zeromqContext;

        public HistoryHandlerTask(BrokerConfiguration configuration, Storage storage)
        {
            _configuration = configuration;
            _storage = storage;
            _repAddress = String.Format("tcp://*:{0}", configuration.ConsumerPort);
        }

        public void Run(CancellationToken token)
        {
            using (Socket repSocket = CreateSocket(ZMQ.SocketType.REP))
            {
                // Bind to socket
                repSocket.Bind(_repAddress);

                // Process while canellation not requested
                while (!token.IsCancellationRequested)
                {
                    // Waits for messages
                    var data = repSocket.Recv();
                    if (data == null) continue;

                    using (var stream = new MemoryStream(data))
                    using (var reader = new BinaryReader(stream))
                    {
                        var request = LoadMessagesRequest.ReadFromStream(stream, reader);

                        if (!_storage.ValidatePartitionNumber(request.Topic, request.Partition))
                            continue;

                        _storage.Insure(request.Topic);

                        var block = _storage.ReadMessagesBlock(request.Topic, request.Partition, request.Offset, request.BlockSize);

                        var response = new AvailableMessagesResponse();
                        response.Data = block.Data;

                        using (var stream2 = new MemoryStream())
                        using (var writer = new AvailableMessagesResponseWriter(stream2))
                        {
                            writer.WriteRequest(response);

                            var binary = stream2.ToArray();
                            repSocket.Send(binary);
                        }
                    }

/*                    var result = Encoding.UTF8.GetString(data);
                    result = "Answer:" + result;

                    repSocket.Send(Encoding.UTF8.GetBytes(result));
 */
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