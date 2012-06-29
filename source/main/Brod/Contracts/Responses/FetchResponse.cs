using System.IO;
using Brod.Common;

namespace Brod.Contracts.Responses
{
    public class FetchResponse : Response
    {
        public byte[] Data { get; set; }

        public FetchResponse() : base(ResponseType.FetchResponse)
        {
        }

        public static FetchResponse ReadFromStream(BinaryStream stream)
        {
            var request = new FetchResponse();
            var length = stream.Reader.ReadInt32();
            request.Data = stream.Reader.ReadBytes(length);
            return request;            
        }

        public override void WriteToStream(BinaryStream stream)
        {
            stream.Writer.Write((short) ResponseType);
            stream.Writer.Write(Data.Length);
            stream.Writer.Write(Data);
        }
    }
}