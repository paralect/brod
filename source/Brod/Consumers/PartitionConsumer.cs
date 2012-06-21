using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Brod.Requests;
using Brod.Responses;
using Brod.Sockets;

namespace Brod.Consumers
{
    public class PartitionConsumer : IDisposable
    {
        private ConsumerConfiguration _configuration;
        private ZMQ.Context _zeromqContext;
        private Socket _reqSocket;

        public PartitionConsumer(String address, ZMQ.Context zeromqContext)
        {
            var configuration = new ConsumerConfiguration { Address = address };
            Initialize(configuration, zeromqContext);
        }

        public PartitionConsumer(ConsumerConfiguration configuration, ZMQ.Context zeromqContext)
        {
            Initialize(configuration, zeromqContext);
        }

        private void Initialize(ConsumerConfiguration configuration, ZMQ.Context zeromqContext)
        {
            _configuration = configuration;
            _zeromqContext = zeromqContext;
            _reqSocket = CreateSocket(ZMQ.SocketType.REQ);

            // Bind to socket
            _reqSocket.Connect(configuration.Address, CancellationToken.None);            
        }

        public IEnumerable<Message> Load(String topic, Int32 partition, Int32 offset, Int32 blockSize)
        {
            var request = new LoadMessagesRequest();
            request.Topic = topic;
            request.Partition = partition;
            request.Offset = offset;
            request.BlockSize = blockSize;

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                request.WriteToStream(stream, writer);

                var data = stream.ToArray();
                //Console.WriteLine("Sending {0} bytes", data.Length);
                _reqSocket.Send(data);
            }

            var result = _reqSocket.Recv();

            using (var reader = new AvailableMessagesResponseReader((new MemoryStream(result))))
            {
                var response = reader.ReadRequest();

                using (var messageReader = new MessageReader(new MemoryStream(response.Data)))
                {
                    foreach (var message in messageReader.ReadAllMessages())
                        yield return message;
                }
                
            }

        }

        private Socket CreateSocket(ZMQ.SocketType socketType)
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