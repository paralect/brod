using System;
using System.IO;
using System.Text;
using Brod.Brokers;
using Brod.Storage;
using Machine.Specifications;

namespace Brod.Tests.Specs.StorageArea.contexts
{
    public class storage_with_sample_data
    {
        Establish context = () =>
        {
            temporaryDirectory = Path.Combine(Path.GetTempPath(), "BrodStorages", Guid.NewGuid().ToString());
            messageString = "Hello, world!";
            messageBytes = Encoding.UTF8.GetBytes(messageString);

            storage = new Store(new BrokerConfiguration() { StorageDirectory = temporaryDirectory });
            storage.Append("test", 0, messageBytes);
            storage.Append("test", 0, messageBytes);
            storage.Append("test", 0, messageBytes);
            storage.Append("test", 0, messageBytes);
            storage.Flush();
        };

        Cleanup cleanup = () =>
            Directory.Delete(temporaryDirectory);

        public static string temporaryDirectory;
        public static Store storage;
        public static string messageString;
        public static byte[] messageBytes;
    }
}