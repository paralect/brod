using System;
using System.Collections.Generic;

namespace Brod.Consumers
{
    public class Consumer
    {
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

        public Consumer(String brokerAddress, ConsumerContext context)
        {
            _context = context;
            _configuration.Address = brokerAddress;
        }

        public ConsumerMessageStream OpenMessageStream(String topic)
        {
            var connector = new ConsumerConnector(_configuration, _context.ZeromqContext);
            var streams = connector.CreateMessageStreams(topic, 1);
            return streams[0];
        }

        public List<ConsumerMessageStream> OpenMessageStream(String topic, Int32 numberOfStreams)
        {
            var connector = new ConsumerConnector(_configuration, _context.ZeromqContext);
            var streams = connector.CreateMessageStreams(topic, numberOfStreams);
            return streams;
        }


    }
}