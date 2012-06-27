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
            using (var consumer = new Consumer("localhost:5568"))
            {
                consumer.StateStorageDirectory = @"c:\tmp\consumer-state";

                using (var stream = consumer.OpenStream("sample-topic"))
                {
                    Console.WriteLine("SampleConsumer.");
                    Console.WriteLine("---------------");
                    Console.WriteLine("  State Storage Directory: {0}", consumer.StateStorageDirectory);
                    Console.WriteLine("  This app will consumer all messages from topic 'sample-topic' and will");
                    Console.WriteLine("  print them line by line.");
                    Console.WriteLine();

                    foreach (var message in stream.NextString())
                    {
                        Console.WriteLine(message);
                    }
                }
                
            }
        }
    }
}
