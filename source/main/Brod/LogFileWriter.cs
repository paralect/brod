using System;
using System.Collections.Generic;
using System.IO;

namespace Brod
{
    public class LogFileWriter : IDisposable
    {
        private readonly FileInfo _fileInfo;
        private readonly FileStream _fileStream;

        public LogFileWriter(string logFilePath)
        {
            _fileInfo = new FileInfo(logFilePath);
            _fileStream = OpenForWrite();
        }

        public void Append(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length == 0)
                throw new ArgumentException("Empty byte array not allowed.");

            _fileStream.Seek(0, SeekOrigin.End);

            using (var writer = new BinaryWriter(_fileStream))
            {
                writer.Write(data);
            }
        }

        FileStream OpenForWrite()
        {
            // we allow concurrent reading
            // no more writers are allowed
            return _fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        }

        public void Dispose()
        {
            if (_fileStream != null)
                _fileStream.Dispose();
        }
    }
}