using System.IO;
using Brod.Common;

namespace Brod.Contracts.Responses
{
    public enum ResponseType : short
    {
        AppendResponse = 1,
        FetchResponse = 2,
        BrokerInfoResponse = 3
    }

    public abstract class Response
    {
        public ResponseType ResponseType { get; set; }

        protected Response(ResponseType requestType)
        {
            ResponseType = requestType;
        }

        public abstract void WriteToStream(BinaryStream stream);
    }
}