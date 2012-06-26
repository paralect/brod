using System;
using System.Globalization;
using System.IO;

namespace Brod.Store
{
    public class Segment : IDisposable
    {
        private readonly Int64 _offset;
        private readonly String _segmentFilePath;
        private FileStream _writeStream;
        private FileStream _readStream;

        public Int64 Offset
        {
            get { return _offset; }
        }

        public Segment(String logDirectory, Int64 offset)
        {
            _offset = offset;
            _segmentFilePath = Path.Combine(logDirectory, GetSegmentFileName(offset));

            Init();
        }

        private void Init()
        {
            if (!File.Exists(_segmentFilePath))
                CreateEmptyFile(_segmentFilePath);
        }

        public void Append(byte[] payload)
        {
            if (_writeStream == null)
            {
                _writeStream = File.Open(_segmentFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                _writeStream.Seek(0, SeekOrigin.End);
            }

            var messageWriter = new MessageWriter(_writeStream);
            messageWriter.WriteMessage(payload);
        }

        /// <summary>
        /// Read <param name="blockLength" /> bytes, starting from  <param name="offset" /> byte from 
        /// specified <param name="topic"/> and <param name="partition" />
        /// </summary>
        public MessagesBlock ReadMessagesBlock(Int32 offset, Int32 blockLength)
        {
            if (_readStream == null)
            {
                _readStream = File.Open(_segmentFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }

            _readStream.Seek(offset, SeekOrigin.Begin);

            var block = new MessagesBlock();
            block.Data = new byte[blockLength];
            block.Length = _readStream.Read(block.Data, 0, blockLength);
            return block;
        }

        public void Flush(bool flushOnDisk)
        {
            if (_writeStream != null)
                _writeStream.Flush(flushOnDisk);
        }
        

        /// <summary>
        /// Creates empty file with specified path and name
        /// </summary>
        private void CreateEmptyFile(string filePath)
        {
            if (!File.Exists(filePath))
                using (File.Create(filePath)) { }
        }

        private String GetSegmentFileName(Int64 offset)
        {
            var fileName = offset.ToString(CultureInfo.InvariantCulture);
            fileName = String.Format("{0}.brod", fileName.PadLeft(20, '0'));
            return fileName;
        }

        public void Dispose()
        {
            if (_writeStream != null)
                _writeStream.Dispose();

            if (_readStream != null)
                _readStream.Dispose();
        }
    }
}