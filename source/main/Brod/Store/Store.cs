using System;
using System.Collections.Generic;
using System.IO;

namespace Brod.Store
{
    /// <summary>
    /// Brod storage
    /// </summary>
    public class Store : IDisposable
    {
        /// <summary>
        /// Broker configuration
        /// </summary>
        private readonly BrokerConfiguration _configuration;

        /// <summary>
        /// Topics, currently serving by this storage.
        /// We are cashing them to prevent filesystem access.
        /// </summary>
        private readonly Dictionary<String, Topic> _topics = new Dictionary<String, Topic>();

        /// <summary>
        /// Creates Store
        /// </summary>
        public Store(BrokerConfiguration configuration)
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

            foreach (var topicFileName in topics)
            {
                var topicName = new DirectoryInfo(topicFileName).Name;
                GetTopic(topicName);
            }
        }

        /// <summary>
        /// Append payload to specified topic and partition
        /// </summary>
        public void Append(String topic, Int32 partition, byte[] payload)
        {
            GetTopic(topic)
                .GetLog(partition)
                .Append(payload);
        }

        /// <summary>
        /// Read <param name="blockLength" /> bytes, starting from  <param name="offset" /> byte from 
        /// specified <param name="topic"/> and <param name="partition" />
        /// </summary>
        public MessagesBlock ReadMessagesBlock(String topic, Int32 partition, Int32 offset, Int32 blockLength)
        {
            return GetTopic(topic)
                .GetLog(partition)
                .ReadMessagesBlock(offset, blockLength);
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

        /// <summary>
        /// Calculates number of partitions for specified topic
        /// </summary>
        public Int32 GetNumberOfPartitionsForTopic(String topic)
        {
            // Get number of partitions for specified topic
            Int32 partitions;
            if (!_configuration.NumberOfPartitionsPerTopic.TryGetValue(topic, out partitions))
                partitions = _configuration.NumberOfPartitions;

            return partitions;
        }

        /// <summary>
        /// Returns Topic for specified name.
        /// Will be created, if not exists.
        /// </summary>
        private Topic GetTopic(String topicName)
        {
            Topic topic;
            if (!_topics.TryGetValue(topicName, out topic))
            {
                topic = new Topic(
                    _configuration.StorageDirectory,
                    topicName,
                    GetNumberOfPartitionsForTopic(topicName));

                _topics[topicName] = topic;
            }

            return topic;
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
            //lock (_openedStreamsPerPath)
            {
                foreach (var topicPair in _topics)
                    topicPair.Value.Flush(flushToDisk);
            }
        }

        public void Dispose()
        {
            foreach (var topicPair in _topics)
            {
                var topic = topicPair.Value;

                if (topic != null)
                    topic.Dispose();
            }
                
        }
    }
}