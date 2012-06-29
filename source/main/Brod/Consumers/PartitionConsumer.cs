using System;
using System.Collections.Generic;
using System.Globalization;
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
            _reqSocket.Connect(Protocolize(configuration.Address), CancellationToken.None);            
        }

        /// <summary>
        /// Return Partition -> Message
        /// </summary>
        public IEnumerable<Tuple<Int32, Message>> Load(String topic, Dictionary<Int32, Int32> offsetByPartition, Int32 blockSize)
        {
            var multifetch = new MultiFetchRequest();
            multifetch.FetchRequests = new List<FetchRequest>(offsetByPartition.Count);
            foreach (var pair in offsetByPartition)
            {
                var request = new FetchRequest();
                request.Topic = topic;
                request.Partition = pair.Key;
                request.Offset = pair.Value;
                request.BlockSize = blockSize;
                multifetch.FetchRequests.Add(request);
            }

            using (var buffer = new BinaryMemoryStream())
            {
                multifetch.WriteToStream(buffer);

                var data = buffer.ToArray();
                _reqSocket.Send(data);
            }

            var result = _reqSocket.Recv();

            using(var buffer = new BinaryMemoryStream(result))
            {
                var responseType = buffer.Reader.ReadInt16();
                var response1 = MultiFetchResponse.ReadFromStream(buffer);

                foreach (var fetchResponse in response1.FetchResponses)
                {
                    using (var messageReader = new MessageReader(new BinaryMemoryStream(fetchResponse.Data)))
                    {
                        foreach (var message in messageReader.ReadAllMessages())
                            yield return new Tuple<int, Message>(fetchResponse.Partition, message);
                    }                    
                }
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