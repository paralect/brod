using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Brod.Common;
using Brod.Messages;

namespace Brod.Contracts.Requests
{
    public class AppendRequest : Request
    {
        public String Topic { get; set; }
        public Int32 Partition { get; set; }
        public List<Message> Messages { get; set; }

        public AppendRequest() : base(RequestType.AppendRequest)
        {
            Messages = new List<Message>();
        }

        public AppendRequest(String topic, Int32 partition, Message message) : base(RequestType.AppendRequest)
        {
            Topic = topic;
            Partition = partition;
            Messages = new List<Message>(1) { message };
        }

        public static AppendRequest ReadFromStream(BinaryStream buffer)
        {
            var request = new AppendRequest();
            request.Topic = buffer.Reader.ReadString();
            request.Partition = buffer.Reader.ReadInt32();
            request.Messages = new MessageReader(buffer).ReadAllMessages().ToList();
            return request;
        }

        public override void WriteToStream(BinaryStream buffer)
        {
            buffer.Writer.Write(Topic);
            buffer.Writer.Write(Partition);
            new MessageWriter(buffer).WriteMessage(Messages);
        }
    }
}