using System;
using Brod.Common;

namespace Brod.Contracts.Requests
{
    public class BrokerInfoRequest : Request
    {
        public BrokerInfoRequest() : base(RequestType.BrokerInfoRequest) { }

        public static BrokerInfoRequest ReadFromStream(BinaryStream buffer)
        {
            var request = new BrokerInfoRequest();
            return request;
        }

        public override void WriteToStream(BinaryStream buffer)
        {
            buffer.Writer.Write((short) RequestType);
        }
    }
}