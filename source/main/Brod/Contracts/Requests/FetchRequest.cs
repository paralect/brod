using System;
using System.IO;
using Brod.Common;

namespace Brod.Contracts.Requests
{
    public class FetchRequest : Request
    {
        public String Topic { get; set; }
        public Int32 Partition { get; set; }
        public Int32 Offset { get; set; }
        public Int32 BlockSize { get; set; }

        public FetchRequest() : base(RequestType.FetchRequest)
        {
        }

        public static FetchRequest ReadFromStream(BinaryStream buffer)
        {
            var request = new FetchRequest();
            request.Topic = buffer.Reader.ReadString();
            request.Partition = buffer.Reader.ReadInt32();
            request.Offset = buffer.Reader.ReadInt32();
            request.BlockSize = buffer.Reader.ReadInt32();
            return request;            
        }

        public override void WriteToStream(BinaryStream buffer)
        {
            buffer.Writer.Write((short)RequestType.FetchRequest);
            buffer.Writer.Write(Topic);
            buffer.Writer.Write(Partition);
            buffer.Writer.Write(Offset);
            buffer.Writer.Write(BlockSize);            
        }
    }
}