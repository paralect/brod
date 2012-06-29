using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Brod.Brokers
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
        /// The id of the broker. This must be set to a unique integer for each broker.
        /// </summary>
        public Int32 BrokerId { get; set; }

        /// <summary>
        /// Hostname the broker will advertise to consumers
        /// </summary>
        public String HostName { get; set; }

        /// <summary>
        /// Port that accepts request from producers (Async)
        /// Default is 5567
        /// </summary>
        public Int32 Port { get; set; }

        /// <summary>
        /// Port that accepts requests from consumers (Sync request-reply)
        /// Default is 5568
        /// </summary>
        public Int32 PullPort { get; set; }

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
        public BrokerConfiguration()
        {
            StorageDirectory = Path.Combine(Path.GetTempPath(), "brod");

            BrokerId = 0;
            HostName = Dns.GetHostName();

            Port = 5567;
            PullPort = 5568;

            NumberOfPartitions = 1;
            NumberOfPartitionsPerTopic = new Dictionary<String, Int32>();
        }

        /// <summary>
        /// Read configuration from BrokerConfigurationSection
        /// </summary>
        public static BrokerConfiguration FromConfigurationSection(BrokerConfigurationSection section)
        {
            var config = new BrokerConfiguration();

            if (!String.IsNullOrWhiteSpace(section.StorageDirectory.Value))
                config.StorageDirectory = section.StorageDirectory.Value;

            if (section.BrokerId.Value != 0)
                config.BrokerId = section.BrokerId.Value;

            if (!String.IsNullOrWhiteSpace(section.HostName.Value))
                config.HostName = section.HostName.Value;

            if (section.Port.Value != 0)
                config.Port = section.Port.Value;            

            if (section.PullPort.Value != 0)
                config.PullPort = section.PullPort.Value;

            if (section.NumberOfPartitions.Value != 0)
                config.NumberOfPartitions = section.NumberOfPartitions.Value;

            if (section.NumberOfPartitionsPerTopic != null)
            {
                foreach (var element in section.NumberOfPartitionsPerTopic)
                {
                    var item = (TopicElement)element;
                    config.NumberOfPartitionsPerTopic.Add(item.Topic, item.Partitions);
                }
            }

            return config;
        }
    }
}