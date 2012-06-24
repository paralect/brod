using System;
using System.Diagnostics;
using Brod.Producers;
using NUnit.Framework;

namespace Brod.Tests.Tests
{
    [TestFixture]
    public class ProducerTest
    {
        [Test]
        public void DoIt()
        {
            var context = new ProducerContext();
            var producer = context.CreateProducer("localhost:5567");
            var stream = producer.OpenMessageStream("test");

            var totalBytesSent = 0;
            const int messageSize = 200;

            var watch = Stopwatch.StartNew();

            for (var i = 0; i < 100000; i++)
            {
                if (i % 1000 == 0)
                    Console.WriteLine("{0})", i);

                var data = new byte[messageSize];
                stream.Send(data);
                totalBytesSent += Message.CalculateOnDiskMessageLength(data.Length);
            }

            stream.Send("end!//");

            watch.Stop();
            Console.WriteLine("Done in {0} msec. {1} bytes sent.", watch.ElapsedMilliseconds, totalBytesSent);
        }
    }
}