using System.IO;

namespace Brod.Contracts.Responses
{
    public class AvailableMessagesResponse : Response
    {
        public byte[] Data { get; set; }

        public static AvailableMessagesResponse ReadFromStream(Stream stream, BinaryReader reader)
        {
            var request = new AvailableMessagesResponse();
            var length = reader.ReadInt32();
            request.Data = reader.ReadBytes(length);
            return request;            
        }

        public override void WriteToStream(Stream stream, BinaryWriter writer)
        {
            writer.Write(Data.Length);
            writer.Write(Data);
        }
    }
}