using System;
using System.Collections.Generic;
using System.IO;
using Brod.Configuration;

namespace Brod
{
    /// <summary>
    /// Storage configuration
    /// </summary>
    public class BrokerConfiguration
    {
        /// <summary>
        /// Path to storage directory. Can be relative to brod.exe.
        /// Default to YOUR_TEMP_FOLDER\brod
        /// </summary>
        public String StorageDirectory { get; set; }

        /// <summary>
        /// Port that accepts request from producers (Async)
        /// Default is 5567
        /// </summary>
        public Int32 ProducerPort { get; set; }
        public const Int32 DefaultProducerPort = 5567;

        /// <summary>
        /// Port that accepts requests from consumers (Sync request-reply)
        /// Default is 5568
        /// </summary>
        public Int32 ConsumerPort { get; set; }
        public const Int32 DefaultConsumerPort = 5568;

        /// <summary>
        /// Default number of partitions for topics that doesn't registered in NumberOfPartitionsPerTopic.
        /// Default is 1
        /// </summary>
        public Int32 NumberOfPartitions { get; set; }
        public const Int32 DefaultNumberOfPartitions = 1;

        /// <summary>
        /// Number of partitions per topic name
        /// </summary>
        public Dictionary<String, Int32> NumberOfPartitionsPerTopic { get; set; }

        /// <summary>
        /// Creates BrokerConfiguration with default settings
        /// </summary>
        public BrokerConfiguration()
        {
            StorageDirectory = Path.Combine(Path.GetTempPath(), "brod");

            ProducerPort = DefaultProducerPort;
            ConsumerPort = DefaultConsumerPort;

            NumberOfPartitions = DefaultNumberOfPartitions;
            NumberOfPartitionsPerTopic = new Dictionary<string, int>();
        }

        public static BrokerConfiguration FromConfigurationSection(BrokerConfigurationSection section)
        {
            var config = new BrokerConfiguration();

            if (!String.IsNullOrWhiteSpace(section.StorageDirectory.Value))
                config.StorageDirectory = section.StorageDirectory.Value;

            if (section.ConsumerPort.Value != 0)
                config.ConsumerPort = section.ConsumerPort.Value;

            if (section.ProducerPort.Value != 0)
                config.ProducerPort = section.ProducerPort.Value;

            if (section.NumberOfPartitions.Value != 0)
                config.NumberOfPartitions = section.NumberOfPartitions.Value;

            if (section.NumberOfPartitionsPerTopic != null)
            {
                foreach (var element in section.NumberOfPartitionsPerTopic)
                {
                    var item = (AcmeInstanceElement)element;
                    config.NumberOfPartitionsPerTopic.Add(item.Topic, item.Partitions);
                }
            }

            return config;
        }
    }
}