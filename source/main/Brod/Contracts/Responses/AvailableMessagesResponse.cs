using System.IO;
using Brod.Common;

namespace Brod.Contracts.Responses
{
    public class AvailableMessagesResponse : Response
    {
        public byte[] Data { get; set; }

        public static AvailableMessagesResponse ReadFromStream(BinaryStream stream)
        {
            var request = new AvailableMessagesResponse();
            var length = stream.Reader.ReadInt32();
            request.Data = stream.Reader.ReadBytes(length);
            return request;            
        }

        public override void WriteToStream(BinaryStream stream)
        {
            stream.Writer.Write(Data.Length);
            stream.Writer.Write(Data);
        }
    }
}