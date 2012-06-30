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
        private RequestSender _sender;

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

            _sender = new RequestSender(configuration.Address, ZMQ.SocketType.REQ, _zeromqContext);
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

            var response = (MultiFetchResponse) _sender.Send(multifetch);

            foreach (var fetchResponse in response.FetchResponses)
            {
                using (var messageReader = new MessageReader(new BinaryMemoryStream(fetchResponse.Data)))
                {
                    foreach (var message in messageReader.ReadAllMessages())
                        yield return new Tuple<int, Message>(fetchResponse.Partition, message);
                }
            }
        }

        public void Dispose()
        {
            if (_sender != null)
                _sender.Dispose();
        }
    }
}