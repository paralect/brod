using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Brod
{
    /// <summary>
    /// Brod storage
    /// </summary>
    public class Storage
    {
        private readonly BrokerConfiguration _configuration;

        public Storage(BrokerConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Initialize storage for specified topic
        /// </summary>
        public void Initialize(String topic)
        {
            // Get number of partitions for specified topic
            Int32 partitions = GetNumberOfPartitionsForTopic(topic);

            // Create each partition
            for (int i = 0; i < partitions; i++)
            {
                InitializePartitionDirectory(topic, i);
            }
        }

        public void Append(String topic, Int32 partition)
        {
            var filePath = GetLogFilePath(topic, partition, 0);

            var stream = new LogFile(filePath);

            stream.Append(Encoding.UTF8.GetBytes("Long life Brod!"));

            var messages = stream.ReadRecords(0).ToList();

            foreach (var message in messages)
            {
                Console.WriteLine(Encoding.UTF8.GetString(message.Payload));
            }

            Console.ReadKey();

        }

        private Int32 GetNumberOfPartitionsForTopic(String topic)
        {
            // Get number of partitions for specified topic
            Int32 partitions;
            if (!_configuration.NumberOfPartitionsPerTopic.TryGetValue(topic, out partitions))
                partitions = _configuration.NumberOfPartitions;

            return partitions;
        }

        /// <summary>
        /// Creates partition directory and empty files for log files.
        /// </summary>
        private void InitializePartitionDirectory(String topic, Int32 partition)
        {
            var partitionDirectoryPath = GetPartitionDirectoryPath(topic, partition);
            Directory.CreateDirectory(partitionDirectoryPath);

            var logFilePath = GetLogFilePath(topic, partition, 0);
            CreateEmptyFile(logFilePath);
        }

        /// <summary>
        /// Creates empty file with specified path and name
        /// </summary>
        private void CreateEmptyFile(string filePath)
        {
            if (!File.Exists(filePath))
                using (File.Create(filePath)) { }
        }

        #region Path utils

        public String GetPartitionDirectoryPath(String topic, Int32 partition)
        {
            return Path.Combine(_configuration.StorageDirectory, topic + "-" + partition);
        }

        public String GetLogFilePath(String topic, Int32 partition, Int64 offset)
        {
            return String.Format("{0}\\{1}",
                GetPartitionDirectoryPath(topic, partition),
                GetLogFileName(offset));
        }

        private String GetLogFileName(Int64 offset)
        {
            var fileName = offset.ToString(CultureInfo.InvariantCulture);
            fileName = String.Format("{0}.brod", fileName.PadLeft(20, '0'));
            return fileName;
        }

        #endregion
    }
}