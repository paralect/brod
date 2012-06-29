using Brod.Common;

namespace Brod.Contracts.Requests
{
    public enum RequestType : short
    {
        AppendRequest = 1,
        FetchRequest = 2,
        BrokerInfoRequest = 3
    }

    public abstract class Request
    {
        public RequestType RequestType { get; set; }

        protected Request(RequestType requestType)
        {
            RequestType = requestType;
        }

        public abstract void WriteToStream(BinaryStream buffer);
    }
}