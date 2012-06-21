using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Brod.Requests
{
    public class LoadMessagesRequest
    {
        public String Topic { get; set; }
        public Int32 Partition { get; set; }
        public Int32 Offset { get; set; }
        public Int32 BlockSize { get; set; }
    }

    public class LoadMessagesRequestReader : IDisposable
    {
        private readonly Stream _stream;
        private readonly BinaryReader _reader;

        public LoadMessagesRequestReader(Stream stream)
        {
            _stream = stream;
            _reader = new BinaryReader(stream);
        }

        public LoadMessagesRequest ReadRequest()
        {
            var request = new LoadMessagesRequest();
            request.Topic = _reader.ReadString();
            request.Partition = _reader.ReadInt32();
            request.Offset = _reader.ReadInt32();
            request.BlockSize = _reader.ReadInt32();
            return request;
        }

        public void Dispose()
        {
            if (_reader != null)
                _reader.Dispose();
        }
    }

    public class LoadMessagesRequestWriter : IDisposable
    {
        private readonly Stream _output;
        private readonly BinaryWriter _writer;

        public LoadMessagesRequestWriter(Stream output)
        {
            _output = output;
            _writer = new BinaryWriter(output, Encoding.UTF8);
        }

        public void WriteRequest(LoadMessagesRequest request)
        {
            _writer.Write(request.Topic);
            _writer.Write(request.Partition);
            _writer.Write(request.Offset);
            _writer.Write(request.BlockSize);
        }

        public void Dispose()
        {
            if (_writer != null)
                _writer.Dispose();
        }
    }
}