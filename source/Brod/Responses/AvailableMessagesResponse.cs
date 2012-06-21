using System;
using System.IO;

namespace Brod.Responses
{
    public class AvailableMessagesResponse
    {
        public byte[] Data { get; set; }
    }

    public class AvailableMessagesResponseReader : IDisposable
    {
        private readonly Stream _stream;
        private readonly BinaryReader _reader;

        public AvailableMessagesResponseReader(Stream stream)
        {
            _stream = stream;
            _reader = new BinaryReader(stream);
        }

        public AvailableMessagesResponse ReadRequest()
        {
            var request = new AvailableMessagesResponse();
            var length = _reader.ReadInt32();
            request.Data = _reader.ReadBytes(length);
            return request;
        }

        public void Dispose()
        {
            if (_reader != null)
                _reader.Dispose();
        }
    }

    public class AvailableMessagesResponseWriter : IDisposable
    {
        private readonly Stream _output;
        private readonly BinaryWriter _writer;

        public AvailableMessagesResponseWriter(Stream output)
        {
            _output = output;
            _writer = new BinaryWriter(output);
        }

        public void WriteRequest(AvailableMessagesResponse request)
        {
            _writer.Write(request.Data.Length);
            _writer.Write(request.Data);
        }

        public void Dispose()
        {
            if (_writer != null)
                _writer.Dispose();
        }
    }
}