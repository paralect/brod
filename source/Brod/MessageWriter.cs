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
            WriteMessage(Message.CreateMessage(payload));
        }

        public void WriteMessage(Message message)
        {
            _writer.Write(message.MessageLength);  // 4 bytes signed integer value
            _writer.Write((byte) 1);        // 1 byte "magic" number. Currently it is always '1'.
            _writer.Write(message.Crc);     // 4 bytes CRC32 hash
            _writer.Write(message.Payload); // Actual message data
        }

        public void Dispose()
        {
            if (_writer != null)
                _writer.Dispose();
        }
    }
}