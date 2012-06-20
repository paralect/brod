using System;
using System.IO;

namespace Brod
{
    public class MessageWriter : IDisposable
    {
        private readonly BinaryWriter _writer;

        public MessageWriter(Stream output)
        {
            _writer = new BinaryWriter(output);
        }

        public void WriteMessage(byte[] payload)
        {
            using (var crc32 = new Crc32())
            {
                var message = new Message();
                message.Magic = 1;
                message.Length = payload.Length + 1 + 4;
                message.Payload = payload;
                message.Crc = crc32.ComputeHash(payload);

                WriteMessage(message);
            }
        }

        public void WriteMessage(Message message)
        {
            _writer.Write(message.Payload.Length + 1 + 4);  // 4 bytes signed integer value
            _writer.Write((byte) 1); // 1 byte "magic" number. Currently it is always '1'.
            _writer.Write(message.Crc);  // 4 bytes CRC32 hash
            _writer.Write(message.Payload);
        }

        public void Dispose()
        {
            if (_writer != null)
                _writer.Dispose();
        }
    }
}