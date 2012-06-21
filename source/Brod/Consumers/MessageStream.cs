using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZMQ;

namespace Brod.Consumers
{
    public class MessageStream
    {
        private readonly ConsumerConfiguration _configuration;
        private readonly Context _context;
        // TODO: just for testing...
        private Int32 offset = 0;

        public String Topic { get; set; }
        public List<Int32> Partitions { get; set; }
        public ConcurrentQueue<Message> Messages { get; set; }

        public MessageStream(ConsumerConfiguration configuration, ZMQ.Context context)
        {
            _configuration = configuration;
            _context = context;
            Messages = new ConcurrentQueue<Message>();
        }

        public void Start()
        {
            var task = Task.Factory.StartNew(() =>
            {
                var consumer = new PartitionConsumer(_configuration.Address, _context);

                while(true)
                {
                    // If we already have enough messages in queue - wait 
                    if (Messages.Count > 100)
                        continue;

                    var result = consumer.Load(Topic, Partitions[0], offset, 300);

                    foreach (var message in result)
                    {
                        offset += Message.CalculateOnDiskMessageLength(message.Payload.Length);
                        Messages.Enqueue(message);
                    }

                    Thread.Sleep(200);
                }
            });
        }

        public IEnumerable<Message> NextMessage()
        {
            while(true)
            {
                Message result;
                if (!Messages.TryDequeue(out result))
                {
                    Thread.Sleep(10);
                    continue;
                }

                yield return result;
            }
        }
    }


}