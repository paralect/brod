using System;
using Brod.Producers;

namespace SampleProducer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var context = new ProducerContext();
            var producer = context.CreateProducer("localhost:5567");
            
            using(var stream = producer.OpenMessageStream("sample-topic"))
            {
                Console.WriteLine("SampleProducer.");
                Console.WriteLine("---------------");
                Console.WriteLine("  Type some text, and press Enter. Doing this you are sending ");
                Console.WriteLine("  messages to broker.");
                Console.WriteLine();

                while (true)
                {
                    String input = Console.ReadLine();
                    stream.Send(input);
                }
            }
        }
    }
}
