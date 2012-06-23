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
            var server = new Broker(new BrokerConfiguration() { StorageDirectory = @"c:\tmp\brod", NumberOfPartitions = 5 });
            server.Start();

            return;

            var message = Encoding.UTF8.GetBytes("Hello, world!");

            var storage = new Storage(new BrokerConfiguration() { StorageDirectory = @"c:\tmp\brod" });

            storage.Append("test", 0, message);
            storage.Append("test", 0, message);
            storage.Append("test", 0, message);
            storage.Append("test", 0, message);

            var messages = storage.ReadMessages("test", 0, 0, 67);

            foreach (var message1 in messages)
            {
                Console.WriteLine(Encoding.UTF8.GetString(message1.Payload));
            }


            return;

            var dir = @"c:\tmp\brod";
            var topic = "test";
            var partitions = 3;

            Initialize(dir, topic, partitions);

            Console.WriteLine("Hello");
        }


        public static void Initialize(String dir, String topic, Int32 partitions)
        {
            for (int i = 0; i < partitions; i++)
            {
                var partitionDir = Path.Combine(dir, topic + "-" + i);
                Directory.CreateDirectory(partitionDir);

                var fileName = CreateLogName(0);
                CreateEmptyFile(Path.Combine(partitionDir, fileName));
            }
        }

        public static void CreateEmptyFile(string filename)
        {
            if (!File.Exists(filename))
                using (File.Create(filename)) { }
        }

        public static String CreateLogName(Int64 offset)
        {
            var fileName = offset.ToString(CultureInfo.InvariantCulture);
            fileName = String.Format("{0}.brod", fileName.PadLeft(20, '0'));
            return fileName;
        }

        public static String CreateLogPath(String dir, String topic, Int32 partition, Int64 offset)
        {
            return String.Format("{0}\\{1}", Path.Combine(dir, topic + "-" + partition), CreateLogName(offset));
        }
    }
}