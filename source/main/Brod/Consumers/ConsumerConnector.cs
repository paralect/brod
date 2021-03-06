﻿using System;
using System.Collections.Generic;
using Brod.Contracts.Responses;

namespace Brod.Consumers
{
    public class ConsumerConnector
    {
        private readonly string _stateStorageDirectory;
        private readonly string _brokerAddress;
        private readonly BrokerInfoResponse _configuration;
        private readonly ZMQ.Context _context;

        public ConsumerConnector(String stateStorageDirectory, String brokerAddress, BrokerInfoResponse configuration, ZMQ.Context context)
        {
            _stateStorageDirectory = stateStorageDirectory;
            _brokerAddress = brokerAddress;
            _configuration = configuration;
            _context = context;
        }

        public Dictionary<String, List<ConsumerMessageStream>> CreateMessageStreams(Dictionary<String, Int32> topicToStreamCount)
        {
            ValidateTopicToStreamDictionary(topicToStreamCount);

            var result = new Dictionary<String, List<ConsumerMessageStream>>();

            foreach (var pair in topicToStreamCount)
                result[pair.Key] = BuildStreamsForTopic(pair.Key, pair.Value);

            return result;
        }

        public List<ConsumerMessageStream> CreateMessageStreams(String topic, Int32 numberOfStreams)
        {
            ValidateStreamNumber(topic, numberOfStreams);

            return BuildStreamsForTopic(topic, numberOfStreams);
        }

        private List<ConsumerMessageStream> BuildStreamsForTopic(String topic, Int32 numberOfStreams)
        {
            var list = new List<ConsumerMessageStream>();
            var partitionsNumber = GetNumberOfPartitionsForTopic(topic);

            var additional = ((partitionsNumber % numberOfStreams) == 0) ? 0 : 1;
            var step = partitionsNumber / numberOfStreams + additional;

            for (int i = 0; i < numberOfStreams; i++)
            {
                var startingFrom = i * step;
                var count = step;

                if (startingFrom + count > partitionsNumber)
                    count = partitionsNumber - startingFrom;

                var messageStream = new ConsumerMessageStream(_stateStorageDirectory, _brokerAddress, _configuration, _context);
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
}