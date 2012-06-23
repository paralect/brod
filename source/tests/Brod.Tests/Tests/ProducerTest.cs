using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Brod.Tests.Tests
{
    [TestFixture]
    public class ProducerTest
    {
        [Test]
        public void DoIt()
        {
            var producer = new Brod.Producers.Producer("tcp://localhost:5567", new ZMQ.Context(1));

            var totalBytesSend = 0;
            var messageSize = 200;

            var watch = Stopwatch.StartNew();

            for (var i = 0; i < 100000; i++)
            {
                if (i % 1000 == 0)
                    Console.WriteLine("{0})", i);

                var data = new byte[messageSize];
                producer.Send("test", 0, data);
                totalBytesSend += Message.CalculateOnDiskMessageLength(data.Length);
            }

            producer.Send("test", 0, "end!//");

            watch.Stop();
            Console.WriteLine("Done in {0} msec. {1} bytes sent.", watch.ElapsedMilliseconds, totalBytesSend);
        }
    }
}