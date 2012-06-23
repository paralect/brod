using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Producer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var producer = new Brod.Producers.Producer("tcp://localhost:5567", new ZMQ.Context(1));

            while (true)
            {
                String input = Console.ReadLine();
                producer.Send("test", 0, input);
            }
        }
    }
}
