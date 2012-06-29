using System.Collections.Generic;
using Brod.Common;

namespace Brod.Contracts.Responses
{
    public class MultiFetchResponse : Response
    {
        public List<FetchResponse> FetchResponses { get; set; }

        public MultiFetchResponse(): base(ResponseType.MultiFetchResponse) { }

        public static MultiFetchResponse ReadFromStream(BinaryStream stream)
        {
            var request = new MultiFetchResponse();
            var length = stream.Reader.ReadInt32();

            request.FetchResponses = new List<FetchResponse>(length);
            for (int i = 0; i < length; i++)
                request.FetchResponses.Add(FetchResponse.ReadFromStream(stream));
            
            return request;
        }

        public override void WriteToStream(BinaryStream stream)
        {
            stream.Writer.Write((short)ResponseType);
            
            stream.Writer.Write(FetchResponses.Count);
            foreach (var fetchResponse in FetchResponses)
                fetchResponse.WriteToStream(stream);
        }
    }
}