using System;
using System.IO;

namespace Brod
{
    public class MessageReader : IDisposable
    {
        private readonly Stream _stream;
        private readonly BinaryReader _reader;

        public MessageReader(Stream stream)
        {
            _stream = stream;
            _reader = new BinaryReader(stream);
        }

        /// <summary>
        /// Message that was correctly read, or null if end of stream found
        /// </summary>
        public Message ReadMessage()
        {
            try
            {
                // If this is the end of stream, return null
                if (_stream.Position == _stream.Length)
                    return null;

                // Can we read 4 bytes?
                if (_stream.Position + 4 /* size of message length field */ > _stream.Length)
                    return null;

                // Read 4 bytes that contains message length 
                var messageLength = _reader.ReadInt32();
                if (messageLength == 0)
                    return null;

                // Can we read (messageLength - 4) bytes?
                if (_stream.Position + messageLength - 4 > _stream.Length)
                    return null;

                var message = new Message();
                message.Magic = _reader.ReadByte();
                message.Crc = _reader.ReadBytes(4);
                message.Payload = _reader.ReadBytes(Message.CalculatePayloadSize(messageLength));

                // Validate message content
                message.Validate();

                return message;
            }
            catch (EndOfStreamException exception)
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (_reader != null)
                _reader.Dispose();
        }
    }
}