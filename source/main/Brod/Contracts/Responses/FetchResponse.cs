using System;
using System.IO;
using Brod.Common;

namespace Brod.Contracts.Responses
{
    public class FetchResponse : Response
    {
        public Int32 Partition { get; set; }
        public byte[] Data { get; set; }

        public FetchResponse() : base(ResponseType.FetchResponse)
        {
        }

        public static FetchResponse ReadFromStream(BinaryStream stream)
        {
            var request = new FetchResponse();
            request.Partition = stream.Reader.ReadInt32();
            var length = stream.Reader.ReadInt32();
            request.Data = stream.Reader.ReadBytes(length);
            return request;            
        }

        public override void WriteToStream(BinaryStream stream)
        {
            stream.Writer.Write(Partition);
            stream.Writer.Write(Data.Length);
            stream.Writer.Write(Data);
        }
    }
}