using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrodConsumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var consumer = new Brod.Consumers.Consumer("tcp://localhost:5568", new ZMQ.Context(1));

            while (true)
            {
                String input = Console.ReadLine();
                var result = consumer.Load("test", 0, 0, 7000);

                foreach (var message in result)
                {
                    Console.WriteLine(Encoding.UTF8.GetString(message.Payload));
                }
            }
        }
    }
}
