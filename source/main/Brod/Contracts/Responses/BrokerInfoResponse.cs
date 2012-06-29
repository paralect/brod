using System;
using System.Collections.Generic;
using Brod.Common;

namespace Brod.Contracts.Responses
{
    public class BrokerInfoResponse : Response
    {
        /// <summary>
        /// Default number of partitions for topics that doesn't registered in NumberOfPartitionsPerTopic.
        /// </summary>
        public Int32 NumberOfPartitions { get; set; }

        /// <summary>
        /// Number of partitions per topic name
        /// </summary>
        public Dictionary<String, Int32> NumberOfPartitionsPerTopic { get; set; }

        public BrokerInfoResponse()
        {
            NumberOfPartitionsPerTopic = new Dictionary<string, int>();
            NumberOfPartitions = 1;
        }

        public static BrokerInfoResponse ReadFromStream(BinaryStream stream)
        {
            var request = new BrokerInfoResponse();
            request.NumberOfPartitions = stream.Reader.ReadInt32();

            // Reading dictionary of <String, Int32>
            var length = stream.Reader.ReadInt32();
            for (int i = 0; i < length; i++)
            {
                var topic = stream.Reader.ReadString();
                var partitionsNumber = stream.Reader.ReadInt32();
                request.NumberOfPartitionsPerTopic.Add(topic, partitionsNumber);
            }
                
            return request;
        }

        public override void WriteToStream(BinaryStream stream)
        {
            stream.Writer.Write(NumberOfPartitions);

            // Writing dictionary of <String, Int32>
            stream.Writer.Write(NumberOfPartitionsPerTopic.Count);
            foreach (var pair in NumberOfPartitionsPerTopic)
            {
                stream.Writer.Write(pair.Key);
                stream.Writer.Write(pair.Value);
            }
        }
    }
}