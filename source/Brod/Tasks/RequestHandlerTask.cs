using System;
using System.IO;
using System.Text;
using System.Threading;
using Brod.Requests;
using Brod.Sockets;

namespace Brod.Tasks
{
    public class RequestHandlerTask : ITask
    {
        private readonly BrokerConfiguration _configuration;
        private readonly Storage _storage;
        private readonly string _pullAddress;
        private ZMQ.Context _zeromqContext;

        public RequestHandlerTask(BrokerConfiguration configuration, Storage storage)
        {
            _configuration = configuration;
            _storage = storage;
            _pullAddress = String.Format("tcp://*:{0}", configuration.ProducerPort);
        }

        public void Run(CancellationToken token)
        {
            using (Socket processPullSocket = CreateSocket(ZMQ.SocketType.PULL))
            {
                // Bind to socket
                processPullSocket.Bind(_pullAddress);

                // Process while canellation not requested
                while (!token.IsCancellationRequested)
                {
                    // Waits for messages
                    var data = processPullSocket.Recv();
                    if (data == null) continue;

                    using (var stream1 = new MemoryStream(data))
                    using (var reader = new BinaryReader(stream1))
                    {
                        var request = AppendMessagesRequest.ReadFromStream(stream1, reader);

                        if (!_storage.ValidatePartitionNumber(request.Topic, request.Partition))
                            continue;

                        _storage.Insure(request.Topic);

                        for (int i = 0; i < request.Messages.Count; i++)
                        {
                            var message = request.Messages[i];
                            _storage.Append(request.Topic, request.Partition, message.Payload);
                        }

                        Console.WriteLine("Request received for Topic: {0} and Partition: {1}. {2} message(s) saved.",
                            request.Topic, request.Partition, request.Messages.Count);
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