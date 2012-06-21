using System;
using System.Collections.Generic;

namespace Brod.Consumers
{
    public class ConsumerConfiguration
    {
        /// <summary>
        /// Broker's "History" endpoint
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// Default number of partitions for topics that doesn't registered in NumberOfPartitionsPerTopic.
        /// Default is 1
        /// </summary>
        public Int32 NumberOfPartitions { get; set; }

        /// <summary>
        /// Number of partitions per topic name
        /// </summary>
        public Dictionary<String, Int32> NumberOfPartitionsPerTopic { get; set; }

        /// <summary>
        /// Creates BrokerConfiguration with default settings
        /// </summary>
        public ConsumerConfiguration()
        {
            NumberOfPartitions = 1;
            NumberOfPartitionsPerTopic = new Dictionary<string, int>();
        }
    }
}