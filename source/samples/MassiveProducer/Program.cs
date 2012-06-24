using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Brod;
using Brod.Producers;

namespace MassiveProducer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var context = new ProducerContext();
            var producer = context.CreateProducer("localhost:5567");
            var stream = producer.OpenMessageStream("test");

            var totalBytesSend = 0;
            const int messageSize = 200;

            var watch = Stopwatch.StartNew();

            for (var i = 0; i < 200000; i++)
            {
                if (i % 1000 == 0)
                    Console.WriteLine("{0})", i);

                var data = new byte[messageSize];
                stream.Send(data);
                totalBytesSend += Message.CalculateOnDiskMessageLength(data.Length);
            }

            stream.Send("end!//");

            watch.Stop();
            Console.WriteLine("Done in {0} msec. {1} bytes sent.", watch.ElapsedMilliseconds, totalBytesSend);
            Console.ReadKey();
        }
    }
}
