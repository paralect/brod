using System;
using System.Collections.Generic;
using System.IO;
using Brod.Common;

namespace Brod.Messages
{
    public class MessageReader : IDisposable
    {
        private readonly BinaryStream _buffer;
        private readonly Stream _stream;
        private readonly BinaryReader _reader;

        public MessageReader(BinaryStream buffer)
        {
            _buffer = buffer;
            _stream = buffer.Stream;
            _reader = buffer.Reader;
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

                // If message length less or equlal zero - we probably read all messages
                if (messageLength <= 0)
                    return null;

                // Can we read messageLength bytes?
                if (_stream.Position + messageLength > _stream.Length)
                    return null;

                var message = new Message();
                message.Magic = _reader.ReadByte();
                message.Crc = _reader.ReadBytes(4);
                message.Payload = _reader.ReadBytes(Message.CalculatePayloadLength(messageLength));

                // Validate message content
                message.Validate();

                return message;
            }
            catch (EndOfStreamException)
            {
                return null;
            }
        }

        /// <summary>
        /// Read all messages from this message stream
        /// </summary>
        public IEnumerable<Message> ReadAllMessages()
        {
            Message message;
            while ((message = ReadMessage()) != null)
                yield return message;
        }

        public void Dispose()
        {
            if (_reader != null)
                _reader.Dispose();
        }
    }
}