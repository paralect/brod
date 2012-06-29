using System;
using System.Diagnostics;
using Brod.Messages;
using Brod.Producers;
using NUnit.Framework;

namespace Brod.Tests.Tests
{
    [TestFixture]
    public class ProducerTest
    {
        [Ignore("Performance test")]
        public void DoIt()
        {
            var producer = new Producer("localhost:5567");

            var totalBytesSent = 0;
            const int messageSize = 200;

            var watch = Stopwatch.StartNew();

            for (var i = 0; i < 100000; i++)
            {
                if (i % 1000 == 0)
                    Console.WriteLine("{0})", i);

                var data = new byte[messageSize];
                producer.Send("test", data);
                totalBytesSent += Message.CalculateOnDiskMessageLength(data.Length);
            }

            producer.Send("test", "end!//");

            watch.Stop();
            Console.WriteLine("Done in {0} msec. {1} bytes sent.", watch.ElapsedMilliseconds, totalBytesSent);
        }
    }
}