using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Brod;

namespace MassiveProducer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var producer = new Brod.Producers.Producer("tcp://localhost:5567", new ZMQ.Context(1));

            var totalBytesSend = 0;
            const int messageSize = 1024;

            var watch = Stopwatch.StartNew();

            for (var i = 0; i < 200000; i++)
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
            Console.ReadKey();
        }
    }
}
