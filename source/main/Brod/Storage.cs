using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Brod.Utilities;

namespace Brod
{
    /// <summary>
    /// Brod storage
    /// </summary>
    public class Storage : IDisposable
    {
        /// <summary>
        /// Topics, currently serving by this storage.
        /// We are cashing them to prevent filesystem access.
        /// </summary>
        private readonly List<String> _topics = new List<string>();

        /// <summary>
        /// Opened filestreams for partition log files.
        /// </summary>
        private readonly Dictionary<String, FileStream> _openedStreamsPerPath = new Dictionary<string, FileStream>();

        /// <summary>
        /// LogFileDescriptor -> LogFilePath map, to cache log file path construction
        /// </summary>
        private readonly HybridDictionary _logFilePathByDescriptor = new HybridDictionary();

        /// <summary>
        /// Broker configuration
        /// </summary>
        private readonly BrokerConfiguration _configuration;

        /// <summary>
        /// Constructs Storage with specified BrokerConfiguration
        /// </summary>
        public Storage(BrokerConfiguration configuration)
        {
            _configuration = configuration;
            Init();
        }

        /// <summary>
        /// Builds internal cache of topics, that this storage contains
        /// </summary>
        private void Init()
        {
            if (!Directory.Exists(_configuration.StorageDirectory))
                Directory.CreateDirectory(_configuration.StorageDirectory);

            var topics = Directory.GetDirectories(_configuration.StorageDirectory);

            foreach (var topic in topics)
            {
                var info = new DirectoryInfo(topic);
                var topicName = info.Name;
                InsureTopicOnDisk(topicName);
            }
        }

        /// <summary>
        /// Initialize storage for specified topic
        /// </summary>
        private void InsureTopicOnDisk(String topic)
        {
            if (_topics.Contains(topic))
                return;

            _topics.Add(topic);

            Int32 partitions = GetNumberOfPartitionsForTopic(topic);

            // Create each partition
            for (int i = 0; i < partitions; i++)
            {
                InitializePartitionDirectory(topic, i);
            }
        }

        /// <summary>
        /// Flush to OS page cache (data will be visible for consumers)
        /// </summary>
        public void Flush()
        {
            InternalFlush(false);
        }

        /// <summary>
        /// Flush on disk to store data permanently
        /// </summary>
        public void FlushOnDisk()
        {
            InternalFlush(true);
        }

        /// <summary>
        /// Flush streams to page cache or disk, depending on <param name="flushToDisk"/> param.
        /// </summary>
        private void InternalFlush(Boolean flushToDisk)
        {
            lock (_openedStreamsPerPath)
            {
                foreach (var openStreamPair in _openedStreamsPerPath)
                {
                    openStreamPair.Value.Flush(flushToDisk);
                }
            }
        }

        /// <summary>
        /// Append payload to specified topic and partition
        /// </summary>
        public void Append(String topic, Int32 partition, byte[] payload)
        {
            InsureTopicOnDisk(topic);

            var logFilePath = GetLogFilePath(topic, partition, 0);

            FileStream fileStream;

            lock (_openedStreamsPerPath)
            {
                if (!_openedStreamsPerPath.TryGetValue(logFilePath, out fileStream))
                {
                    var logFile = new LogFile(logFilePath);
                    fileStream = logFile.OpenForWrite();
                    fileStream.Seek(0, SeekOrigin.End);
                    _openedStreamsPerPath[logFilePath] = fileStream;
                }
            }

            var messageWriter = new MessageWriter(fileStream);
            messageWriter.WriteMessage(payload);
        }

        /// <summary>
        /// Read <param name="blockLength" /> bytes, starting from  <param name="offset" /> byte from 
        /// specified <param name="topic"/> and <param name="partition" />
        /// </summary>
        public MessagesBlock ReadMessagesBlock(String topic, Int32 partition, Int32 offset, Int32 blockLength)
        {
            InsureTopicOnDisk(topic);

            var logFilePath = GetLogFilePath(topic, partition, 0);
            var logFile = new LogFile(logFilePath);

            using (var fileStream = logFile.OpenForRead())
            {
                fileStream.Seek(offset, SeekOrigin.Begin);

                var block = new MessagesBlock();
                block.Data = new byte[blockLength];
                block.Length = fileStream.Read(block.Data, 0, blockLength);
                return block;
            }
        }

        /// <summary>
        /// Read and parse all message that can be found in <param name="blockLength" /> bytes, starting from 
        /// <param name="offset" /> byte from specified <param name="topic"/> and <param name="partition" />
        /// </summary>
        public IEnumerable<Message> ReadMessages(String topic, Int32 partition, Int32 offset, Int32 blockLength)
        {
            var block = ReadMessagesBlock(topic, partition, offset, blockLength);
            
            foreach (var message in block.ReadMessages())
                yield return message;
        }

        public Int32 GetNumberOfPartitionsForTopic(String topic)
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

            if (!Directory.Exists(partitionDirectoryPath))
                Directory.CreateDirectory(partitionDirectoryPath);

            var logFilePath = GetLogFilePath(topic, partition, 0);

            if (!File.Exists(logFilePath))
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

        /// <summary>
        /// Returns true of partition number is valid for current broker configuration
        /// </summary>
        public bool ValidatePartitionNumber(String topic, Int32 partition)
        {
            var partitionsCount = GetNumberOfPartitionsForTopic(topic);
            if (partition >= partitionsCount)
            {
                Console.WriteLine("Invalid request received for Topic: {0} and Partition: {1}. " +
                    "For topic {0} only {2} partitions available on server.",
                    topic, partition, partitionsCount);

                return false;
            }

            return true;
        }

        #region Path utils

        public String GetPartitionDirectoryPath(String topic, Int32 partition)
        {
            var partitionText = partition
                .ToString(CultureInfo.InvariantCulture)
                .PadLeft(4, '0');

            return Path.Combine(_configuration.StorageDirectory, topic, "partition-" + partitionText);
        }

        public String GetLogFilePath(String topic, Int32 partition, Int64 offset)
        {
            var descriptor = new LogFileDescriptor() { Topic = topic, Partition = partition, Offset = offset };

            string path = _logFilePathByDescriptor[descriptor] as string;

            if (path == null)
            {
                path = String.Format("{0}\\{1}",
                    GetPartitionDirectoryPath(topic, partition),
                    GetLogFileName(offset));

                _logFilePathByDescriptor[descriptor] = path;
            }

            return path;

            

        }

        private String GetLogFileName(Int64 offset)
        {
            var fileName = offset.ToString(CultureInfo.InvariantCulture);
            fileName = String.Format("{0}.brod", fileName.PadLeft(20, '0'));
            return fileName;
        }

        #endregion

        public void Dispose()
        {
            if (_openedStreamsPerPath != null)
            {
                foreach (var fileStreamPair in _openedStreamsPerPath)
                {
                    if (fileStreamPair.Value != null)
                        fileStreamPair.Value.Dispose();
                }
            }
        }
    }

    public struct LogFileDescriptor
    {
        public String Topic { get; set; }
        public Int32 Partition { get; set; }
        public Int64 Offset { get; set; }

        public override bool Equals(object obj)
        {
            var descriptor = (LogFileDescriptor) obj;
            if (String.CompareOrdinal(Topic, descriptor.Topic) != 0)
                return false;

            if (Partition != descriptor.Partition)
                return false;

            if (Offset != descriptor.Offset)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return HashCodeUtils.Compute(Topic, Partition, Offset);
        }
    }
}