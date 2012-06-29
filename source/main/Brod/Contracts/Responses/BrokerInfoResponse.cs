using System;
using System.Collections.Generic;
using Brod.Common;

namespace Brod.Contracts.Responses
{
    public class BrokerInfoResponse : Response
    {
        /// <summary>
        /// Broker hostname
        /// </summary>
        public String HostName { get; set; }

        /// <summary>
        /// The id of the broker
        /// </summary>
        public Int32 BrokerId { get; set; }

        /// <summary>
        /// Port, that accepts only oneway (unidirectional) requests.
        /// </summary>
        public Int32 PullPort { get; set; }

        /// <summary>
        /// Default number of partitions for topics that doesn't registered in NumberOfPartitionsPerTopic.
        /// </summary>
        public Int32 NumberOfPartitions { get; set; }

        /// <summary>
        /// Number of partitions per topic name
        /// </summary>
        public Dictionary<String, Int32> NumberOfPartitionsPerTopic { get; set; }

        public BrokerInfoResponse() : base(ResponseType.BrokerInfoResponse)
        {
            NumberOfPartitionsPerTopic = new Dictionary<string, int>();
        }

        public static BrokerInfoResponse ReadFromStream(BinaryStream stream)
        {
            var request = new BrokerInfoResponse();
            request.HostName = stream.Reader.ReadString();
            request.BrokerId = stream.Reader.ReadInt32();
            request.PullPort = stream.Reader.ReadInt32();
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
            stream.Writer.Write((short)ResponseType);

            stream.Writer.Write(HostName);
            stream.Writer.Write(BrokerId);
            stream.Writer.Write(PullPort);
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