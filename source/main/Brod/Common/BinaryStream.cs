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

    /// <summary>
    /// A BinaryMemoryStream represents a sequence of zero or more bytes that can be written to or read from, 
    /// and which expands automatically as necessary to accomodate any bytes written to it.
    /// </summary>
    public class BinaryMemoryStream : BinaryStream
    {
        public new MemoryStream Stream
        {
            get { return (MemoryStream) _stream; }
        }

        public BinaryMemoryStream()
        {
            _stream = new MemoryStream();
        }

        public BinaryMemoryStream(byte[] bytes)
        {
            _stream = new MemoryStream(bytes);
        }

        /// <summary>
        /// Very memory intensive operation
        /// </summary>
        public byte[] ToArray()
        {
            return Stream.ToArray();
        }        
    }
}