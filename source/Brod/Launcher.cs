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
            var dir = @"c:\tmp\brod";
            var topic = "test";
            var partitions = 3;

            Initialize(dir, topic, partitions);

            AppendMessage(dir, topic, 0);
//            AppendMessage(dir, topic, 1);
//            AppendMessage(dir, topic, 1);

            Console.WriteLine("Hello");
        }

        public static void AppendMessage(String dir, String topic, Int32 partition)
        {
            var filePath = CreateLogPath(dir, topic, partition, 0);

            var stream = new LogFile(filePath);
            stream.Append(Encoding.UTF8.GetBytes("Long life Brod!"));

            var messages = stream.ReadRecords(0).ToList();

            foreach (var message in messages)
            {
                Console.WriteLine(Encoding.UTF8.GetString(message.Payload));
            }

            Console.ReadKey();

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