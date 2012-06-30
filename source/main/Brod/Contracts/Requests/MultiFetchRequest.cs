using System.Collections.Generic;
using Brod.Common;

namespace Brod.Contracts.Requests
{
    public class MultiFetchRequest : Request
    {
        public List<FetchRequest> FetchRequests { get; set; }

        public MultiFetchRequest() : base(RequestType.MultiFetchRequest)
        {
        }

        public static MultiFetchRequest ReadFromStream(BinaryStream buffer)
        {
            var result = new MultiFetchRequest();
            var count = buffer.Reader.ReadInt32();

            result.FetchRequests = new List<FetchRequest>(count);
            for(int i = 0; i < count; i++)
            {
                var requestType = buffer.Reader.ReadInt16();
                result.FetchRequests.Add(FetchRequest.ReadFromStream(buffer));
            }
                

            return result;
        }

        public override void WriteToStream(BinaryStream buffer)
        {
            buffer.Writer.Write(FetchRequests.Count);
            foreach (var fetchRequest in FetchRequests)
                fetchRequest.WriteToStream(buffer);
        }
    }
}