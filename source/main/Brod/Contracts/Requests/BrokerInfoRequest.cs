using System;
using Brod.Common;

namespace Brod.Contracts.Requests
{
    public class BrokerInfoRequest : Request
    {
        public BrokerInfoRequest() : base(RequestType.BrokerInfoRequest) { }

        public static BrokerInfoRequest ReadFromStream(BinaryStream buffer)
        {
            return new BrokerInfoRequest();
        }

        public override void WriteToStream(BinaryStream buffer)
        {
        }
    }
}