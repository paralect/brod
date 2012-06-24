using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZMQ;

namespace Brod.Consumers
{
    public class ConsumerMessageStream : IDisposable
    {
        private readonly ConsumerConfiguration _configuration;
        private readonly Context _context;
        private Object _lock = new object();
        private Boolean _started = false;

        private StreamState _streamState = null;
        private ConsumerStateStorage _stateStorage = null;

        public String Topic { get; set; }
        public List<Int32> Partitions { get; set; }
        public ConcurrentQueue<Message> Messages { get; set; }

        public StreamState StreamState
        {
            get { return _streamState; }
        }

        public ConsumerMessageStream(ConsumerConfiguration configuration, ZMQ.Context context)
        {
            _configuration = configuration;
            _context = context;
            Messages = new ConcurrentQueue<Message>();
        }

        private void Start()
        {
            _started = true;
            _stateStorage = new ConsumerStateStorage(_configuration);
            _streamState = _stateStorage.ReadStreamState(Topic, "test-group", Partitions);

            var task = Task.Factory.StartNew(() =>
            {
                var consumer = new PartitionConsumer(_configuration.Address, _context);
                var offset = _streamState.OffsetByPartition[0];

                while(true)
                {
                    // If we already have enough messages in queue - wait 
                    if (Messages.Count > 100)
                        continue;

                    var result = consumer.Load(Topic, Partitions[0], offset, 300);

                    var messageCount = 0;
                    foreach (var message in result)
                    {
                        Messages.Enqueue(message);
                        offset += Message.CalculateOnDiskMessageLength(message.Payload.Length);
                        messageCount++;
                    }

                    // Wait for 700 msecond, if there is no new messages
                    // This value should be configurable
                    if (messageCount == 0)
                        Thread.Sleep(700);
                }
            });
        }

        public IEnumerable<Message> NextMessage()
        {
            if (!_started)
                Start();

            while(true)
            {
                Message result;
                if (!Messages.TryDequeue(out result))
                {
                    Thread.Sleep(10);
                    continue;
                }

                yield return result;

                _streamState.OffsetByPartition[0] += Message.CalculateOnDiskMessageLength(result.Payload.Length);
                _stateStorage.WriteStreamState(_streamState, 0);
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