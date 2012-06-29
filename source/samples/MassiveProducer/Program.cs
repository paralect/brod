using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Brod;
using Brod.Messages;
using Brod.Producers;

namespace MassiveProducer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var producer = new Producer("localhost:5567");

            var totalBytesSend = 0;
            const int messageSize = 1024;
            const int numberOfMessages = 105000;
            const int totalSizeInKb = (numberOfMessages * messageSize) / 1024 / 1024;

            Console.WriteLine("MassiveProducer.");
            Console.WriteLine("---------------");
            Console.WriteLine("  This app will storm broker by producing {0:n0} messages {1:n0} bytes each", numberOfMessages, messageSize);
            Console.WriteLine("  Total size of data that broker will receive - {0:n0} mb.", totalSizeInKb);
            Console.WriteLine("  When broker will receive all messages, it will output 'Done!'");
            Console.WriteLine("  Press Enter when you are ready");
            Console.WriteLine();

            Console.ReadKey();

            var watch = Stopwatch.StartNew();

            for (var i = 0; i < numberOfMessages; i++)
            {
                if (i != 0 && i % 1000 == 0)
                    Console.WriteLine("{0:n0} messages sent", i);

                var data = new byte[messageSize];
                producer.Send("massive-topic", data);
                totalBytesSend += Message.CalculateOnDiskMessageLength(data.Length);
            }

            producer.Send("massive-topic", "end!//");

            watch.Stop();
            Console.WriteLine("Done in {0:n0} msec. {1:n0} bytes sent.", watch.ElapsedMilliseconds, totalBytesSend);
            Console.WriteLine("We can continue our work, but it will take some time, until messages actually delivered to broker.");
            Console.WriteLine("Check your broker storage, it should grow with additional {0:n0} mb.", totalSizeInKb);
            Console.ReadKey();
        }
    }
}
