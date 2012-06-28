using System;
using System.IO;
using System.Text;

namespace Brod.Common
{
    public class BinaryStream : IDisposable
    {
        protected Stream _stream;
        protected BinaryReader _reader;
        protected BinaryWriter _writer;

        public BinaryReader Reader
        {
            get { return _reader ?? (_reader = new BinaryReader(_stream, Encoding.UTF8)); }
        }

        public BinaryWriter Writer
        {
            get { return _writer ?? (_writer = new BinaryWriter(_stream, Encoding.UTF8)); }
        }

        public Stream Stream
        {
            get { return _stream; }
        }

        public BinaryStream(Stream stream)
        {
            _stream = stream;
        }

        protected BinaryStream()
        {
            
        }

        public void Dispose()
        {
            if (_reader != null)
                _reader.Dispose();

            if (_writer != null)
                _writer.Dispose();
        }
    }
}