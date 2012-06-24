using System;
using System.Collections.Generic;
using System.Text;
using Brod.Consumers;

namespace SampleConsumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var context = new ConsumerContext();
            var consumer = context.CreateConsumer("localhost:5568");
            
            using(var stream = consumer.OpenMessageStream("sample-topic"))
            {
                foreach (var message in stream.NextString())
                {
                    Console.WriteLine(message);
                }
            }

            /*
            var connector = new ConsumerConnector(new ConsumerConfiguration() { Address = "tcp://localhost:5568", StateStorageDirectory = @"c:\tmp\state" }, new ZMQ.Context(1));

            var streams = connector.OpenMessageStream(new Dictionary<string, int> { { "test", 1 } });

            var stream = streams["test"][0];

            stream.Start();

            foreach (var message in stream.NextMessage())
            {
                Console.WriteLine(Encoding.UTF8.GetString(message.Payload));
            }
             * */
        }
    }
}
