using System.IO;
using Brod.Common;

namespace Brod.Contracts.Responses
{
    public enum ResponseType : short
    {
        AppendResponse = 1,
        FetchResponse = 2,
        MultiFetchResponse = 3,
        BrokerInfoResponse = 4
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