using System;
using System.Collections.Generic;

namespace Brod.Consumers
{
    public class Consumer : IDisposable
    {
        private readonly static ConsumerContext _sharedContext = new ConsumerContext();

        private readonly ConsumerContext _context;
        private ConsumerConfiguration _configuration = new ConsumerConfiguration();

        /// <summary>
        /// Broker's "History" endpoint
        /// </summary>
        public String Address
        {
            get { return _configuration.Address; }
            set { _configuration.Address = value; }
        }

        /// <summary>
        /// Default number of partitions for topics that doesn't registered in NumberOfPartitionsPerTopic.
        /// Default is 1
        /// </summary>
        public Int32 NumberOfPartitions
        {
            get { return _configuration.NumberOfPartitions; }
            set { _configuration.NumberOfPartitions = value; }
        }

        /// <summary>
        /// Directory where Consumer state will be stored
        /// </summary>
        public String StateStorageDirectory
        {
            get { return _configuration.StateStorageDirectory; }
            set { _configuration.StateStorageDirectory = value; }
        }

        /// <summary>
        /// Number of partitions per topic name
        /// </summary>
        public Dictionary<String, Int32> NumberOfPartitionsPerTopic
        {
            get { return _configuration.NumberOfPartitionsPerTopic;  }
            set { _configuration.NumberOfPartitionsPerTopic = value; }
        }

        public Consumer(String brokerAddress)
        {
            _context = _sharedContext;
            _configuration.Address = "tcp://" + brokerAddress;
        }

        public Consumer(String brokerAddress, ConsumerContext context)
        {
            _context = context;
            _configuration.Address = "tcp://" + brokerAddress;
        }

        /// <summary>
        /// Open stream for specified topic, that has one partition (#0)
        /// </summary>
        public ConsumerMessageStream OpenStream(String topic)
        {
            var connector = new ConsumerConnector(_configuration, _context.ZeromqContext);
            var streams = connector.CreateMessageStreams(topic, 1);
            return streams[0];
        }

        /// <summary>
        /// Open stream for specified topic, that has numberOfPartitions partitions
        /// </summary>
        public ConsumerMessageStream OpenStream(String topic, Int32 numberOfPartitions)
        {
            return null;
        }

        /// <summary>
        /// Open numberOfStreams streams for specified topic, that has numberofPartitions partitions.
        /// Paritions will be assigned to each stream in such a way, that each stream will consume
        /// roughly the same number of partitions.
        /// </summary>
        public List<ConsumerMessageStream> OpenStreams(String topic, Int32 numberOfPartitions, Int32 numberOfStreams)
        {
            return null;
        }

        public List<ConsumerMessageStream> OpenStreams(String topic, Int32 numberOfStreams)
        {
            var connector = new ConsumerConnector(_configuration, _context.ZeromqContext);
            var streams = connector.CreateMessageStreams(topic, numberOfStreams);
            return streams;
        }

        public void Dispose()
        {
            
        }
    }
}