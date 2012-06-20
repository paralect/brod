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
                var result = consumer.Get(Encoding.UTF8.GetBytes(input));

                Console.WriteLine(Encoding.UTF8.GetString(result));
            }
        }
    }
}
