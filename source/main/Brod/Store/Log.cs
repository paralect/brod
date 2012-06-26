using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Brod.Store
{
    public class Log : IDisposable
    {
        private readonly Int32 _partition;
        private readonly String _logDirectory;
        private readonly Dictionary<Int64, Segment> _segments = new Dictionary<Int64, Segment>();

        public int Partition
        {
            get { return _partition; }
        }

        public Log(String topicDirectory, Int32 partition)
        {
            _partition = partition;
            _logDirectory = GetLogDirectoryPath(topicDirectory);

            Init();
        }

        private void Init()
        {
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);

            var segment = new Segment(_logDirectory, 0); //only one segment for now
            _segments.Add(segment.Offset, segment);
        }

        public void Append(byte[] payload)
        {
            GetSegment(0).Append(payload);
        }

        public void Flush(bool flushOnDisk)
        {
            foreach (var segmentPair in _segments)
                segmentPair.Value.Flush(flushOnDisk);
        }


        /// <summary>
        /// Read <param name="blockLength" /> bytes, starting from  <param name="offset" /> byte from 
        /// specified <param name="topic"/> and <param name="partition" />
        /// </summary>
        public MessagesBlock ReadMessagesBlock(Int32 offset, Int32 blockLength)
        {
            return GetSegment(0).ReadMessagesBlock(offset, blockLength);
        }


        private String GetLogDirectoryPath(String topicDirectory)
        {
            var partitionText = _partition
                .ToString(CultureInfo.InvariantCulture)
                .PadLeft(4, '0');

            return Path.Combine(topicDirectory, "partition-" + partitionText);
        }

        private Segment GetSegment(Int64 segmentOffset)
        {
            if (segmentOffset < 0)
                throw new Exception(String.Format("Incorrect segment offset"));

            return _segments[segmentOffset];
        }

        public void Dispose()
        {
            foreach (var segmentPair in _segments)
            {
                var segment = segmentPair.Value;

                if (segment != null)
                    segment.Dispose();
            }            
        }
    }
}