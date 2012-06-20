using System;
using System.IO;
using System.Text;
using Machine.Specifications;

namespace Brod.Tests.Specs.StorageArea.contexts
{
    public class storage_with_sample_data
    {
        Establish context = () =>
        {
            temporaryDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            messageString = "Hello, world!";
            messageBytes = Encoding.UTF8.GetBytes(messageString);

            storage = new Storage(new BrokerConfiguration() { StorageDirectory = temporaryDirectory });
            storage.Initialize("test");

            storage.Append("test", 0, messageBytes);
            storage.Append("test", 0, messageBytes);
            storage.Append("test", 0, messageBytes);
            storage.Append("test", 0, messageBytes);
        };

        Cleanup cleanup = () =>
            Directory.Delete(temporaryDirectory);

        public static string temporaryDirectory;
        public static Storage storage;
        public static string messageString;
        public static byte[] messageBytes;
    }
}