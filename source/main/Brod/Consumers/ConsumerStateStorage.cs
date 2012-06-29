using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Brod.Contracts.Responses;

namespace Brod.Consumers
{
    public class StreamState
    {
        public String Topic { get; set; }
        public String Group { get; set; }
        public Dictionary<Int32, Int32> OffsetByPartition { get; set; }

        public StreamState(string topic, string @group, Dictionary<Int32, Int32> offsetByPartition)
        {
            Topic = topic;
            Group = @group;
            OffsetByPartition = offsetByPartition;
        }
    }

    public class ConsumerStateStorage
    {
        private readonly string _stateStorageDirectory;

        public ConsumerStateStorage(String stateStorageDirectory)
        {
            _stateStorageDirectory = stateStorageDirectory;
        }

        public StreamState ReadStreamState(String topic, String group, List<Int32> partitions)
        {
            if (partitions == null || partitions.Count == 0)
                throw new ArgumentException("Empty partitions set not allowed");

            Dictionary<Int32, Int32> offsetByPartition = new Dictionary<Int32, Int32>(partitions.Count);

            foreach (var partition in partitions)
            {
                var stateFilePath = GetPartitionStateFilePath(topic, group, partition);

                if (!File.Exists(stateFilePath))
                {
                    var dir = GetPartitionDirectoryPath(topic, group, partition);
                    Directory.CreateDirectory(dir);
                    using (File.Create(stateFilePath)) { }
                }
                    

                var offset = File.ReadAllText(stateFilePath);

                offsetByPartition[partition] = String.IsNullOrWhiteSpace(offset) ? 0 : 
                    Int32.Parse(offset, CultureInfo.InvariantCulture);
            }

            return new StreamState(topic, group, offsetByPartition);
        }

        public void WriteStreamState(String topic, String group, Int32 partition, Int32 offset)
        {
            var stateFilePath = GetPartitionStateFilePath(topic, group, partition);

            if (!File.Exists(stateFilePath))
                using (File.Create(stateFilePath)) { }

            File.WriteAllText(stateFilePath, offset.ToString(CultureInfo.InvariantCulture));

        }

        public void WriteStreamState(StreamState state)
        {
            if (state == null)
                throw new ArgumentNullException("state");

            foreach (var pair in state.OffsetByPartition)
                WriteStreamState(state.Topic, state.Group, pair.Key, pair.Value);
        }

        /// <summary>
        /// Write state for only specified partition.
        /// </summary>
        public void WriteStreamState(StreamState state, Int32 partition)
        {
            WriteStreamState(state.Topic, state.Group, partition, state.OffsetByPartition[partition]);
        }

        #region Path utils

        public String GetPartitionDirectoryPath(String topic, String group, Int32 partition)
        {
            var partitionText = partition
                .ToString(CultureInfo.InvariantCulture)
                .PadLeft(4, '0');

            return Path.Combine(_stateStorageDirectory, topic, group, "partition-" + partitionText);
        }

        public String GetPartitionStateFilePath(String topic, String group, Int32 partition)
        {
            return String.Format("{0}\\{1}",
                GetPartitionDirectoryPath(topic, group, partition),
                ".state");
        }

        #endregion
    }
}