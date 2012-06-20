using System;
using System.IO;

namespace Brod
{
    public class MessageReader : IDisposable
    {
        private BinaryReader reader;

        public MessageReader(Stream input)
        {
            reader = new BinaryReader(input);
        }

        /// <summary>
        /// Message that was correctly read, or null if end of stream found
        /// </summary>
        public Message ReadMessage()
        {
            try
            {
                var message = new Message();

                message.Length = reader.ReadInt32();

                if (message.Length == 0)
                    return null;

                message.Magic = reader.ReadByte();
                message.Crc = reader.ReadBytes(4);
                message.Payload = reader.ReadBytes(message.Length + 1 + 4);

                return message;
            }
            catch (EndOfStreamException exception)
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (reader != null)
                reader.Dispose();
        }
    }
}