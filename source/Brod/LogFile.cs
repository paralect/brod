using System;
using System.Collections.Generic;
using System.IO;

namespace Brod
{
    public class LogFile
    {
        readonly FileInfo _data;

        public LogFile(string name)
        {
            _data = new FileInfo(name);
        }

        public IEnumerable<Message> ReadRecords(long offset)
        {
            if (!_data.Exists)
            {
                // file could've been created since the last check
                _data.Refresh();
                if (!_data.Exists)
                {
                    yield break;
                }
            }

            using (var file = OpenForRead())
            {
                var reader = new BinaryReader(file);

                while (true)
                {
                    var readMessage = ReadMessage(reader, file);

                    if (readMessage == null)
                        break;
                    
                    yield return readMessage;
                }
            }
        }

        public Message ReadMessage(BinaryReader reader, FileStream stream)
        {
            if (stream.Position == stream.Length)
                return null;

            var message = new Message();

            message.Length = reader.ReadInt32();
            message.Magic = reader.ReadByte();
            message.Crc = reader.ReadInt32();
            message.Payload = reader.ReadBytes(message.Length - 1 - 4);

            return message;
        }

        public void Append(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length == 0)
                throw new ArgumentException("Buffer must contain at least one byte.");

            using (var fileStream = OpenForWrite())
            {
                fileStream.Seek(0, SeekOrigin.End);

                using (var memoryStream = new MemoryStream())
                using (var writer = new BinaryWriter(fileStream))
                using (var crc32 = new Crc32())
                {
                    writer.Write(data.Length + 1 + 4);
                    writer.Write((byte) 0); // "magic" number
                    writer.Write(crc32.ComputeHash(data));  // 4 bytes CRC32 hash
                    writer.Write(data);

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
        }

        FileStream OpenForWrite()
        {
            // we allow concurrent reading
            // no more writers are allowed
            return _data.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        }

        FileStream OpenForRead()
        {
            // we allow concurrent writing or reading
            return _data.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}