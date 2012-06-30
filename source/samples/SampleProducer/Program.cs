using System;
using System.Net;
using System.Text;
using Brod.Producers;

namespace SampleProducer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using(var producer = new Producer("localhost:5567"))
            {
                Console.WriteLine("SampleProducer.");
                Console.WriteLine("---------------");
                Console.WriteLine("  Type some text, and press Enter. Doing this you are sending ");
                Console.WriteLine("  messages to broker.");
                Console.WriteLine();

                while (true)
                {
                    String input = Console.ReadLine();
                    producer.Send("sample-topic", input);
                }
            }
        }
    }
}
