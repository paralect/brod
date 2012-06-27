using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Brod.Messages;

namespace Brod.Requests
{
    public class AppendMessagesRequest : Request
    {
        public String Topic { get; set; }
        public Int32 Partition { get; set; }
        public List<Message> Messages { get; set; }

        public AppendMessagesRequest() : base(RequestType.AppendMessages)
        {
            Messages = new List<Message>();
        }

        public AppendMessagesRequest(String topic, Int32 partition, Message message) : base(RequestType.AppendMessages)
        {
            Topic = topic;
            Partition = partition;
            Messages = new List<Message>(1) { message };
        }

        public static AppendMessagesRequest ReadFromStream(Stream stream, BinaryReader reader)
        {
            var request = new AppendMessagesRequest();
            request.Topic = reader.ReadString();
            request.Partition = reader.ReadInt32();

            var messageReader = new MessageReader(stream);
            request.Messages = messageReader.ReadAllMessages().ToList();
            return request;
        }

        public void WriteToStream(Stream stream, BinaryWriter writer)
        {
            writer.Write((short) RequestType.AppendMessages);
            writer.Write(Topic);
            writer.Write(Partition);

            var messageWriter = new MessageWriter(stream);
            messageWriter.WriteMessage(Messages);
        }
    }
}