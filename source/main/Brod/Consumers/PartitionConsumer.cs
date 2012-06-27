using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Brod.Common;
using Brod.Contracts.Requests;
using Brod.Contracts.Responses;
using Brod.Messages;
using Brod.Network;

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
            var request = new FetchRequest();
            request.Topic = topic;
            request.Partition = partition;
            request.Offset = offset;
            request.BlockSize = blockSize;

            using (var buffer = new BinaryMemoryStream())
            {
                request.WriteToStream(buffer);

                var data = buffer.ToArray();
                _reqSocket.Send(data);
            }

            var result = _reqSocket.Recv();

            using(var buffer = new BinaryMemoryStream(result))
            {
                var response = AvailableMessagesResponse.ReadFromStream(buffer);

                using (var messageReader = new MessageReader(new BinaryMemoryStream(response.Data)))
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