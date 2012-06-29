using System;
using System.Collections.Generic;
using Brod.Contracts.Requests;
using Brod.Contracts.Responses;
using Brod.Network;

namespace Brod.Consumers
{
    public class Consumer : IDisposable
    {
        private readonly static ConsumerContext _sharedContext = new ConsumerContext();
        private readonly ConsumerContext _context;

        private String _address;
        private String _stateStorageDirectory;
        private BrokerInfoResponse _infoResponse;
        private readonly RequestSender _sender;

        /// <summary>
        /// Directory where Consumer state will be stored
        /// </summary>
        public String StateStorageDirectory
        {
            get { return _stateStorageDirectory; }
            set { _stateStorageDirectory = value; }
        }

        public Consumer(String brokerAddress)
        {
            _context = _sharedContext;
            _address = brokerAddress;

            _sender = new RequestSender(_address, ZMQ.SocketType.REQ, _context.ZmqContext);
            _infoResponse = _sender.Send(new BrokerInfoRequest()) as BrokerInfoResponse;

            if (_infoResponse == null)
                throw new Exception("Cannot create producer, because broker info request was unsuccessfull");

        }

        /// <summary>
        /// Open single stream for specified topic
        /// </summary>
        public ConsumerMessageStream OpenStream(String topic)
        {
            var connector = new ConsumerConnector(_stateStorageDirectory, _address, _infoResponse, _context.ZmqContext);
            var streams = connector.CreateMessageStreams(topic, 1);
            return streams[0];
        }

        /// <summary>
        /// Open numberOfStreams streams for specified topic, that has numberofPartitions partitions.
        /// Paritions will be assigned to each stream in such a way, that each stream will consume
        /// roughly the same number of partitions.
        /// </summary>
        public List<ConsumerMessageStream> OpenStreams(String topic, Int32 numberOfStreams)
        {
            var connector = new ConsumerConnector(_stateStorageDirectory, _address, _infoResponse, _context.ZmqContext);
            var streams = connector.CreateMessageStreams(topic, numberOfStreams);
            return streams;
        }

        public void Dispose()
        {
            
        }
    }
}