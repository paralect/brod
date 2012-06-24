﻿using System;
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
            var stream = producer.OpenMessageStream("massive-topic");

            var totalBytesSend = 0;
            const int messageSize = 1024;
            const int numberOfMessages = 100000;
            const int totalSizeInMb = (numberOfMessages * messageSize) / 1024;

            Console.WriteLine("MassiveProducer.");
            Console.WriteLine("---------------");
            Console.WriteLine("  This app will storm broker by producing {0:n0} messages {1:n0} bytes each", numberOfMessages, messageSize);
            Console.WriteLine("  Total size of data that broker will receive - {0:n0} mb.", totalSizeInMb);
            Console.WriteLine("  Press Enter when you are ready");
            Console.WriteLine();

            Console.ReadKey();

            var watch = Stopwatch.StartNew();

            for (var i = 0; i < numberOfMessages; i++)
            {
                if (i != 0 && i % 1000 == 0)
                    Console.WriteLine("{0:n0} messages sent", i);

                var data = new byte[messageSize];
                stream.Send(data);
                totalBytesSend += Message.CalculateOnDiskMessageLength(data.Length);
            }

            stream.Send("end!//");

            watch.Stop();
            Console.WriteLine("Done in {0:n0} msec. {1:n0} bytes sent.", watch.ElapsedMilliseconds, totalBytesSend);
            Console.WriteLine("We can continue our work, but it will take some time, until messages actually delivered to broker.");
            Console.WriteLine("Check your broker storage, it should grow with additional {0:n0} mb.", totalSizeInMb);
            Console.ReadKey();
        }
    }
}
