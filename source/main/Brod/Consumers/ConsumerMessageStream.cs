using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Brod.Contracts.Responses;
using Brod.Messages;
using Brod.Network;
using ZMQ;

namespace Brod.Consumers
{
    public class ConsumerMessageStream : IDisposable
    {
        private readonly string _stateStorageDirectory;
        private readonly string _brokerAddress;
        private readonly BrokerInfoResponse _configuration;
        private readonly Context _context;
        private Object _lock = new object();
        private Boolean _started = false;

        private StreamState _streamState = null;
        private ConsumerStateStorage _stateStorage = null;

        public String Topic { get; set; }
        public List<Int32> Partitions { get; set; }
        public ConcurrentQueue<Tuple<Int32, Message>> Messages { get; set; }

        public StreamState StreamState
        {
            get { return _streamState; }
        }

        public ConsumerMessageStream(String stateStorageDirectory, String brokerAddress, BrokerInfoResponse configuration, ZMQ.Context context)
        {
            _stateStorageDirectory = stateStorageDirectory;
            _brokerAddress = brokerAddress;
            _configuration = configuration;
            _context = context;
            Messages = new ConcurrentQueue<Tuple<Int32, Message>>();
        }

        private void Start()
        {
            _started = true;
            _stateStorage = new ConsumerStateStorage(_stateStorageDirectory);
            _streamState = _stateStorage.ReadStreamState(Topic, "default-group", Partitions);

            var task = Task.Factory.StartNew(() =>
            {
                var consumer = new PartitionConsumer(_brokerAddress, _context);

                var offsetByPartition = new Dictionary<Int32, Int32>();
                foreach (var pair in _streamState.OffsetByPartition)
                    offsetByPartition.Add(pair.Key, pair.Value);

                while(true)
                {
                    // If we already have enough messages in queue - wait 
                    if (Messages.Count > 100)
                        continue;

                    var result = consumer.Load(Topic, offsetByPartition, 300);

                    var messageCount = 0;
                    foreach (var tuple in result)
                    {
                        Messages.Enqueue(tuple);
                        offsetByPartition[tuple.Item1] += Message.CalculateOnDiskMessageLength(tuple.Item2.Payload.Length);
                        messageCount++;
                    }

                    // Wait for 500 msecond, if there is no new messages
                    // This value should be configurable
                    if (messageCount == 0)
                        Thread.Sleep(500);
                }
            });
        }

        public IEnumerable<Message> NextMessage()
        {
            if (!_started)
                Start();

            while(true)
            {
                Tuple<Int32, Message> result;
                if (!Messages.TryDequeue(out result))
                {
                    Thread.Sleep(10);
                    continue;
                }

                yield return result.Item2;

                _streamState.OffsetByPartition[result.Item1] += Message.CalculateOnDiskMessageLength(result.Item2.Payload.Length);
                _stateStorage.WriteStreamState(_streamState, result.Item1);
            }
        }

        public IEnumerable<String> NextString(Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            foreach (var message in NextMessage())
                yield return encoding.GetString(message.Payload);
        }

        public IEnumerable<String> NextString()
        {
            foreach (var stringValue in NextString(Encoding.UTF8))
                yield return stringValue;
        }


        public void Dispose()
        {
            // 
        }
    }


}