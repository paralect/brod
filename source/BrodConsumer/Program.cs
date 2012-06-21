﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brod;

namespace BrodConsumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var consumer = new Brod.Consumers.PartitionConsumer("tcp://localhost:5568", new ZMQ.Context(1));

            int offset = 0;
            while (true)
            {
                String input = Console.ReadLine();
                var result = consumer.Load("test", 0, offset, 7000);

                foreach (var message in result)
                {
                    offset += Message.CalculateOnDiskMessageLength(message.Payload.Length);

                    Console.WriteLine(Encoding.UTF8.GetString(message.Payload));
                }
            }
        }
    }
}
