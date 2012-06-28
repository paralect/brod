using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Brod.Producers
{
    public class Producer : IDisposable
    {
        /// <summary>
        /// Shared ProducerContext among all Producers
        /// </summary>
        private readonly static ProducerContext _sharedContext = new ProducerContext();

        /// <summary>
        /// Producer context
        /// </summary>
        private readonly ProducerContext _context = _sharedContext;

        /// <summary>
        /// Broker brokerAddress
        /// </summary>
        private readonly string _brokerAddress;

        /// <summary>
        /// Partitioner routes producer requests to selected partition
        /// </summary>
        private IPartitioner _partitioner = new DefaultPartitioner();

        /// <summary>
        /// Default number of partitions for topics that doesn't registered in NumberOfPartitionsPerTopic.
        /// </summary>
        private Int32 _numberOfPartitions = 1;

        /// <summary>
        /// Number of partitions per topic name
        /// </summary>
        private Dictionary<String, Int32> _numberOfPartitionsPerTopic = new Dictionary<string, int>();

        /// <summary>
        /// Partitioner routes producer requests to selected partition.
        /// Default is DefaultPartitioner
        /// </summary>
        public IPartitioner Partitioner
        {
            get { return _partitioner; }
            set { _partitioner = value; }
        }

        /// <summary>
        /// Default number of partitions for topics
        /// Default is 1
        /// </summary>
        public Int32 NumberOfPartitions
        {
            get { return _numberOfPartitions; }
            set { _numberOfPartitions = value; }
        }

        /// <summary>
        /// Number of partitions per topic name
        /// </summary>
        public Dictionary<String, Int32> NumberOfPartitionsPerTopic
        {
            get { return _numberOfPartitionsPerTopic; }
            set { _numberOfPartitionsPerTopic = value; }
        }

        /// <summary>
        /// Constructs Producer with specified broker address
        /// </summary>
        public Producer(String brokerAddress)
        {
            _brokerAddress = Protocolize(brokerAddress);
        }

        /// <summary>
        /// Open stream for specified topic that has one partition (#0)
        /// </summary>
        public ProducerMessageStream OpenStream(String topic)
        {
            return new ProducerMessageStream(_brokerAddress, topic, _sharedContext);
        }

        /// <summary>
        /// Open stream for specified topic that has numberOfParitions partitions.
        /// DefaultPartitioner will be used.
        /// </summary>
        public ProducerMessageStream OpenStream(String topic, Int32 numberOfPartitions)
        {
            return OpenStream(topic, numberOfPartitions, _partitioner);
        }

        /// <summary>
        /// Open stream for specified topic that has numberOfParitions partitions with
        /// specified partitioner
        /// </summary>
        public ProducerMessageStream OpenStream(String topic, Int32 numberOfPartitions, IPartitioner partitioner)
        {
            return null;
        }

        /// <summary>
        /// Insures that protocol is specified. If it doesn't - use tcp://
        /// </summary>
        private String Protocolize(String address)
        {
            if (address.StartsWith("tcp://", false, CultureInfo.InvariantCulture))
                return address;

            return "tcp://" + address;
        }

        public void Dispose()
        {
            // If context not shared - dispose it
            if (_context != _sharedContext && _context != null)
                _context.Dispose();

        }
    }
}