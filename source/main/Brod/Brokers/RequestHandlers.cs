using System;
using System.IO;
using System.Text;
using Brod.Requests;
using Brod.Responses;
using Brod.Storage;

namespace Brod.Brokers
{
    public class RequestHandlers
    {
        private readonly BrokerConfiguration _configuration;
        private readonly Store _storage;

        public RequestHandlers(BrokerConfiguration configuration, Store storage)
        {
            _configuration = configuration;
            _storage = storage;
        }

        public Response Handle(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                var type = (RequestType) reader.ReadInt16();

                switch (type)
                {
                    case RequestType.AppendMessages:
                        HandleAppendMessages(stream, reader);
                        break;

                    case RequestType.LoadMessages:
                        return HandleLoadMessages(stream, reader);

                }
            }

            return null;
        }

        public void HandleAppendMessages(Stream stream, BinaryReader reader)
        {
            var request = AppendMessagesRequest.ReadFromStream(stream, reader);

            if (!_storage.ValidatePartitionNumber(request.Topic, request.Partition))
                return;

            for (int i = 0; i < request.Messages.Count; i++)
            {
                var message = request.Messages[i];

                //For testing purpose only
                if (message.Payload.Length < 20)
                {
                    var text = Encoding.UTF8.GetString(message.Payload);
                    if (text == "end!//")
                        Console.WriteLine("Done!");
                }

                _storage.Append(request.Topic, request.Partition, message.Payload);

                // Flushing to OS cashe
                _storage.Flush();
            }
        }

        public Response HandleLoadMessages(Stream stream, BinaryReader reader)
        {
            var request = LoadMessagesRequest.ReadFromStream(stream, reader);

            if (!_storage.ValidatePartitionNumber(request.Topic, request.Partition))
                return null;

            var block = _storage.ReadMessagesBlock(request.Topic, request.Partition, request.Offset, request.BlockSize);

            var response = new AvailableMessagesResponse();
            response.Data = (block.Length == 0) ? new byte[0] : block.Data;

            return response;
        }
    }
}