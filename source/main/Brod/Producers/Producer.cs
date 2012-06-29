using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Brod.Common;
using Brod.Contracts.Requests;
using Brod.Contracts.Responses;
using Brod.Messages;
using Brod.Network;
using ZMQ;
using Socket = Brod.Network.Socket;

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

        private readonly RequestSender _sender;
        private readonly RequestSender _pushSender;
        private Encoding _encoding = Encoding.UTF8;
        private BrokerInfoResponse _infoResponse;

        /// <summary>
        /// Partitioner routes producer requests to selected partition
        /// </summary>
        private IPartitioner _partitioner = new DefaultPartitioner();

        /// <summary>
        /// Partitioner routes producer requests to selected partition.
        /// Default is DefaultPartitioner
        /// </summary>
        public IPartitioner Partitioner
        {
            get { return _partitioner; }
            set { _partitioner = value; }
        }

        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        /// <summary>
        /// Constructs Producer with specified broker address
        /// </summary>
        public Producer(String brokerAddress)
        {
            _brokerAddress = brokerAddress;
            _sender = new RequestSender(_brokerAddress, SocketType.REQ, _context.ZmqContext);
            _infoResponse = _sender.Send(new BrokerInfoRequest()) as BrokerInfoResponse;

            var pullAddress = String.Format("{0}:{1}", _infoResponse.HostName, _infoResponse.PullPort);
            _pushSender = new RequestSender(pullAddress, SocketType.PUSH, _context.ZmqContext);
        }

        /// <summary>
        /// Send binary message to specified topic. Partition will be selected by Partitioner of this producer.
        /// </summary>
        public void Send(String topic, byte[] payload)
        {
            Send(topic, payload, null, _partitioner);
        }

        /// <summary>
        /// Send binary message to specified topic with specified key. Partition will be selected by Partitioner of this producer.
        /// </summary>
        public void Send(String topic, byte[] payload, Object key)
        {
            Send(topic, payload, key, _partitioner);
        }

        /// <summary>
        /// Send binary message to specified topic with specified key, using specified partitioner.
        /// </summary>
        public void Send(String topic, byte[] payload, Object key, IPartitioner partitioner)
        {
            var partitionsNumber = GetNumberOfPartitionsForTopic(topic);
            var partition = _partitioner.SelectPartition(null, partitionsNumber);
            var request = new AppendRequest(topic, partition, Message.CreateMessage(payload));
            _pushSender.Push(request);
        }

        /// <summary>
        /// Send text message to specified topic, using default UTF-8 encoding. Partition will be selected by Partitioner of this producer.
        /// </summary>
        public void Send(String topic, String message)
        {
            Send(topic, message, _encoding, null, _partitioner);
        }

        /// <summary>
        /// Send text message to specified topic with specified key, using default UTF-8 encoding. Partition will be selected by Partitioner of this producer.
        /// </summary>
        public void Send(String topic, String message, Object key)
        {
            Send(topic, message, _encoding, key, _partitioner);
        }

        /// <summary>
        /// Send text message to specified topic with specified key, using default UTF-8 encoding and specified partitioner
        /// </summary>
        public void Send(String topic, String message, Object key, IPartitioner partitioner)
        {
            Send(topic, message, _encoding, key, partitioner);
        }

        /// <summary>
        /// Send text message to specified topic, using specified encoding. Partition will be selected by Partitioner of this producer.
        /// </summary>
        public void Send(String topic, String message, Encoding encoding)
        {
            Send(topic, message, encoding, null, _partitioner);
        }

        /// <summary>
        /// Send text message to specified topic with specified key, using specified encoding.
        /// </summary>
        public void Send(String topic, String message, Encoding encoding, Object key)
        {
            Send(topic, message, encoding, key, _partitioner);
        }

        /// <summary>
        /// Send text message to specified topic with specified key, using specified encoding and partitioner.
        /// </summary>
        public void Send(String topic, String message, Encoding encoding, Object key, IPartitioner partitioner)
        {
            Send(topic, encoding.GetBytes(message), key, partitioner);
        }

        public Int32 GetNumberOfPartitionsForTopic(String topic)
        {
            // Get number of partitions for specified topic
            Int32 partitions;
            if (!_infoResponse.NumberOfPartitionsPerTopic.TryGetValue(topic, out partitions))
                partitions = _infoResponse.NumberOfPartitions;

            return partitions;
        }

        public void Dispose()
        {
            // If context not shared - dispose it
            if (_context != _sharedContext && _context != null)
                _context.Dispose();

            if (_sender != null)
                _sender.Dispose();

            if (_pushSender != null)
                _pushSender.Dispose();
        }
    }
}