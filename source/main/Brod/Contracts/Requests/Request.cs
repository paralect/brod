namespace Brod.Contracts.Requests
{
    public class Request
    {
        public RequestType RequestType { get; set; }

        public Request(RequestType requestType)
        {
            RequestType = requestType;
        }
    }
}