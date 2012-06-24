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
            var server = new Broker(BrokerConfiguration.FromConfigurationSection(section));
            server.Start();
        }
    }
}