using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Brod.Requests
{
    public class AppendMessagesRequest
    {
        public String Topic { get; set; }
        public Int32 Partition { get; set; }
        public List<Message> Messages { get; set; }

        public AppendMessagesRequest()
        {
            Messages = new List<Message>();
        }

        public AppendMessagesRequest(String topic, Int32 partition, Message message)
        {
            Topic = topic;
            Partition = partition;
            Messages = new List<Message>(1) { message };
        }
    }

    public class AppendMessagesRequestReader : IDisposable
    {
        private readonly Stream _stream;
        private readonly BinaryReader _reader;

        public AppendMessagesRequestReader(Stream stream)
        {
            _stream = stream;
            _reader = new BinaryReader(stream);
        }

        public AppendMessagesRequest ReadRequest()
        {
            var request = new AppendMessagesRequest();
            request.Topic = _reader.ReadString();
            request.Partition = _reader.ReadInt32();

            var messageReader = new MessageReader(_stream);
            request.Messages = messageReader.ReadAllMessages().ToList();
            return request;
        }

        public void Dispose()
        {
            if (_reader != null)
                _reader.Dispose();
        }
    }

    public class AppendMessagesRequestWriter : IDisposable
    {
        private readonly Stream _output;
        private readonly BinaryWriter _writer;

        public AppendMessagesRequestWriter(Stream output)
        {
            _output = output;
            _writer = new BinaryWriter(output, Encoding.UTF8);
        }

        public void WriteRequest(AppendMessagesRequest request)
        {
            _writer.Write(request.Topic);
            _writer.Write(request.Partition);

            var messageWriter = new MessageWriter(_output);
            messageWriter.WriteMessage(request.Messages);
        }

        public void Dispose()
        {
            if (_writer != null)
                _writer.Dispose();
        }
    }
}