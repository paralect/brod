using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Brod.Requests
{
    public class LoadMessagesRequest
    {
        public String Topic { get; set; }
        public Int32 Partition { get; set; }
        public Int32 Offset { get; set; }
        public Int32 BlockSize { get; set; }

        public static LoadMessagesRequest ReadFromStream(Stream stream, BinaryReader reader)
        {
            var request = new LoadMessagesRequest();
            request.Topic = reader.ReadString();
            request.Partition = reader.ReadInt32();
            request.Offset = reader.ReadInt32();
            request.BlockSize = reader.ReadInt32();
            return request;            
        }

        public void WriteToStream(Stream stream, BinaryWriter writer)
        {
            writer.Write((short)RequestType.LoadMessages);
            writer.Write(Topic);
            writer.Write(Partition);
            writer.Write(Offset);
            writer.Write(BlockSize);            
        }
    }
}