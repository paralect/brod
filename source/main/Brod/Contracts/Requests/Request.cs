namespace Brod.Contracts.Requests
{
    public enum RequestType : short
    {
        AppendRequest = 1,
        FetchRequest = 2,
    }

    public class Request
    {
        public RequestType RequestType { get; set; }

        public Request(RequestType requestType)
        {
            RequestType = requestType;
        }
    }
}