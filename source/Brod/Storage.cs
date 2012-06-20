using System;
using System.Collections.Generic;
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

        public void Append(String topic, Int32 partition, byte[] payload)
        {
            var logFilePath = GetLogFilePath(topic, partition, 0);
            var logFile = new LogFile(logFilePath);

            using (var memoryStream = new MemoryStream())
            using (var messageWriter = new MessageWriter(memoryStream))
            using (var fileStream = logFile.OpenForWrite())
            {
                messageWriter.WriteMessage(payload);

                fileStream.Seek(0, SeekOrigin.End);
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.CopyTo(fileStream);
            }
        }

        public MessagesBlock ReadMessagesBlock(String topic, Int32 partition, Int32 offset, Int32 blockSize)
        {
            var logFilePath = GetLogFilePath(topic, partition, 0);
            var logFile = new LogFile(logFilePath);

            using (var fileStream = logFile.OpenForRead())
            {
                var block = new MessagesBlock();
                block.Data = new byte[blockSize];
                block.Length = fileStream.Read(block.Data, offset, blockSize);
                return block;
            }
        }

        public IEnumerable<Message> ReadMessages(String topic, Int32 partition, Int32 offset, Int32 blockSize)
        {
            var block = ReadMessagesBlock(topic, partition, offset, blockSize);
            
            foreach (var message in block.ReadMessages())
                yield return message;
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