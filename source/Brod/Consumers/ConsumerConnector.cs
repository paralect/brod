using System;
using System.Collections.Generic;

namespace Brod.Consumers
{
    public class ConsumerConnector
    {
        private readonly ConsumerConfiguration _configuration;
        private readonly ZMQ.Context _context;

        public ConsumerConnector(ConsumerConfiguration configuration, ZMQ.Context context)
        {
            _configuration = configuration;
            _context = context;
        }

        public Dictionary<String, List<MessageStream>> CreateMessageStreams(Dictionary<String, Int32> topicToStreamCount)
        {
            ValidateTopicToStreamDictionary(topicToStreamCount);

            var result = new Dictionary<String, List<MessageStream>>();

            foreach (var pair in topicToStreamCount)
                result[pair.Key] = BuildStreamsForTopic(pair.Key, pair.Value);

            return result;
        }

        private List<MessageStream> BuildStreamsForTopic(String topic, Int32 numberOfStreams)
        {
            var list = new List<MessageStream>();
            var partitionsNumber = GetNumberOfPartitionsForTopic(topic);

            var additional = ((partitionsNumber % numberOfStreams) == 0) ? 0 : 1;
            var step = partitionsNumber / numberOfStreams + additional;

            for (int i = 0; i < numberOfStreams; i++)
            {
                var startingFrom = i * step;
                var count = step;

                if (startingFrom + count > partitionsNumber)
                    count = partitionsNumber - startingFrom;

                var messageStream = new MessageStream();
                messageStream.Topic = topic;
                messageStream.Partitions = GetPartitions(i*step, count);
                list.Add(messageStream);
            }

            return list;
        }

        private List<Int32> GetPartitions(Int32 fromIncluding, Int32 count)
        {
            List<Int32> partitions = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                partitions.Add(i + fromIncluding);
            }

            return partitions;
        }

        public void ValidateTopicToStreamDictionary(Dictionary<String, Int32> topicToStreamCount)
        {
            foreach (var pair in topicToStreamCount)
                ValidateStreamNumber(pair.Key, pair.Value);
        }

        /// <summary>
        /// Returns true of partition number is valid for current broker configuration
        /// </summary>
        public void ValidateStreamNumber(String topic, Int32 numberOfStreams)
        {
            if (numberOfStreams == 0)
                throw new Exception(String.Format("You attempting to create zero streams for topic {0}. You cannot create zero number of streams.", topic));

            var partitionsCount = GetNumberOfPartitionsForTopic(topic);
            if (numberOfStreams > partitionsCount)
                throw new Exception(String.Format(
                    "Cannot create {0} consumer streams for topic '{1}' because consumer configured to have {2} partition(s) for topic '{1}'",
                    numberOfStreams, topic, partitionsCount));
        }

        public Int32 GetNumberOfPartitionsForTopic(String topic)
        {
            // Get number of partitions for specified topic
            Int32 partitions;
            if (!_configuration.NumberOfPartitionsPerTopic.TryGetValue(topic, out partitions))
                partitions = _configuration.NumberOfPartitions;

            return partitions;
        }
    }

    public class MessageStream
    {
        public String Topic { get; set; }
        public List<Int32> Partitions { get; set; }

        public IEnumerable<Message> NextMessage()
        {
            return null;
        }
    }
}