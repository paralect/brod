using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace Brod.Log
{
    /// <summary>
    /// TreadSafe
    /// </summary>
    public class LogManager
    {
        private readonly BrokerConfiguration _configuration;

        /// <summary>
        /// LogFileDescriptor -> LogFilePath map, to cache log file path construction
        /// </summary>
        private readonly HybridDictionary _logByDescriptor = new HybridDictionary();

        public LogManager(BrokerConfiguration configuration)
        {
            _configuration = configuration;
        }

        private void Init()
        {
            if (!Directory.Exists(_configuration.StorageDirectory))
                Directory.CreateDirectory(_configuration.StorageDirectory);

            var topics = Directory.GetDirectories(_configuration.StorageDirectory);

            foreach (var topic in topics)
            {
                var info = new DirectoryInfo(topic);
                var topicName = info.Name;
                //GetLog(topic)
            }
        }

        private void SetLog(String topic, Int32 partition, Log log)
        {
            var descriptor = new LogDescriptor() { Topic = topic, Partition = partition };
            _logByDescriptor[descriptor] = log;
        }

        private Log GetLog(String topic, Int32 partition)
        {
            var descriptor = new LogDescriptor() { Topic = topic, Partition = partition };
            var log = _logByDescriptor[descriptor] as Log;
            
            if (log == null)
                _logByDescriptor[descriptor] = log = new Log();

            return log;
        }

    }
}