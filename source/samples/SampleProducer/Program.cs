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

            while (true)
            {
                String input = Console.ReadLine();
                producer.Send("sample-topic", 0, input);
            }
        }
    }
}
