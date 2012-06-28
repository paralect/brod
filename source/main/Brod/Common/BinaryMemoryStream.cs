using System.IO;

namespace Brod.Common
{
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