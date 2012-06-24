using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Brod.Configuration;

namespace Brod
{
    public class Launcher
    {
        public static void Main(string[] args)
        {
            var section = (BrokerConfigurationSection) ConfigurationManager.GetSection("brodBroker");
            var configuration = BrokerConfiguration.FromConfigurationSection(section);

            Console.WriteLine("Brod Broker, v0.0.0.0.1");
            Console.WriteLine("-----------------------");
            Console.WriteLine("  Storage Directory: {0}", configuration.StorageDirectory);
            Console.WriteLine("  Default number of partitions: {0}", configuration.NumberOfPartitions);

            var server = new Broker(configuration);
            server.Start();
        }
    }
}