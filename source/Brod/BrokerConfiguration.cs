using System;
using System.Collections.Generic;
using System.IO;

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

        /// <summary>
        /// Port that accepts requests from consumers (Sync request-reply)
        /// Default is 5568
        /// </summary>
        public Int32 ConsumerPort { get; set; }

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
            ProducerPort = 5567;
            ConsumerPort = 5568;
            StorageDirectory = Path.Combine(Path.GetTempPath(), "brod");
            NumberOfPartitions = 1;
            NumberOfPartitionsPerTopic = new Dictionary<string, int>();
        }
    }
}