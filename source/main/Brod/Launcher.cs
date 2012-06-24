using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Brod
{
    public class Launcher
    {
        public static void Main(string[] args)
        {
            var server = new Broker(new BrokerConfiguration()
            {
                StorageDirectory = @"c:\tmp\brod", 
                NumberOfPartitions = 5
            });

            server.Start();
        }
    }
}