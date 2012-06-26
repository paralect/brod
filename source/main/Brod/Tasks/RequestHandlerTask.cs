using System;
using System.IO;
using System.Text;
using System.Threading;
using Brod.Brokers;
using Brod.Network;
using Brod.Requests;
using Brod.Storage;
using Brod.Tasks.Abstract;

namespace Brod.Tasks
{
    public class RequestHandlerTask : ITask
    {
        private readonly BrokerConfiguration _configuration;
        private readonly Store _storage;
        private readonly string _pullAddress;
        private ZMQ.Context _zeromqContext;

        public RequestHandlerTask(BrokerConfiguration configuration, Store storage)
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
                Console.WriteLine("  Listening for incoming connections from producers on port {0}", _configuration.ProducerPort);

                // Process while canellation not requested
                while (!token.IsCancellationRequested)
                {
                    // Waits for messages
                    var data = processPullSocket.Recv();
                    if (data == null)
                    {
                        Console.WriteLine("Idle");
                        continue;
                    }

                    using (var stream = new MemoryStream(data))
                    using (var reader = new BinaryReader(stream))
                    {
                        var request = AppendMessagesRequest.ReadFromStream(stream, reader);

                        if (!_storage.ValidatePartitionNumber(request.Topic, request.Partition))
                            continue;

                        for (int i = 0; i < request.Messages.Count; i++)
                        {
                            var message = request.Messages[i];

                            //For testing purpose only
                            if (message.Payload.Length < 20)
                            {
                                var text = Encoding.UTF8.GetString(message.Payload);
                                if (text == "end!//")
                                    Console.WriteLine("Done!");
                            }

                            _storage.Append(request.Topic, request.Partition, message.Payload);

                            // Flushing to OS cashe
                            _storage.Flush();
                        }

/*                        Console.WriteLine("Request received for Topic: {0} and Partition: {1}. {2} message(s) saved.",
                            request.Topic, request.Partition, request.Messages.Count);*/
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