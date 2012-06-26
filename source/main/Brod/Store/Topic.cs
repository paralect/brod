using System;
using System.Collections.Generic;
using System.IO;

namespace Brod.Store
{
    public class Topic : IDisposable
    {
        private readonly String _topicName;
        private readonly String _topicDirectory;
        private readonly Int32 _numberOfPartitions;

        private readonly Dictionary<Int32, Log> _logs = new Dictionary<int, Log>();

        public String Name
        {
            get { return _topicName; }
        }

        public Topic(String storeDirectory, String topicName, Int32 numberOfPartitions)
        {
            _topicName = topicName;
            _numberOfPartitions = numberOfPartitions;
            _topicDirectory = Path.Combine(storeDirectory, topicName);

            Init();
        }

        private void Init()
        {
            InitializeTopicDirectory();

            for (int i = 0; i < _numberOfPartitions; i++)
            {
                GetLog(i);
            }
        }

        /// <summary>
        /// Creates partition directory and empty files for log files.
        /// </summary>
        private void InitializeTopicDirectory()
        {
            if (!Directory.Exists(_topicDirectory))
                Directory.CreateDirectory(_topicDirectory);
        }

        public Log GetLog(Int32 partitionId)
        {
            if (partitionId < 0 || partitionId >= _numberOfPartitions)
                throw new Exception(String.Format("Incorrect partition id - {0}. Only {1} partitions allowed for topic {2}", partitionId, _numberOfPartitions, _topicName));

            Log partition;
            if (!_logs.TryGetValue(partitionId, out partition))
                _logs[partitionId] = partition = new Log(_topicDirectory, partitionId);

            return partition;
        }
    
        public void Flush(Boolean flushOnDisk)
        {
            foreach (var logPair in _logs)
                logPair.Value.Flush(flushOnDisk);
        }

        public void Dispose()
        {
            foreach (var logPair in _logs)
            {
                var log = logPair.Value;

                if (log != null)
                    log.Dispose();
            }
        }
    }
}