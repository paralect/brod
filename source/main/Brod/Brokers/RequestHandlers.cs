using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Brod.Common;
using Brod.Contracts.Requests;
using Brod.Contracts.Responses;
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

        /// <summary>
        /// Map request to the handler function of the following signature:
        /// Response SomeHandler(BinaryStream)
        /// </summary>
        public Func<BinaryStream, Response> MapHandlers(RequestType requestType, BinaryStream buffer)
        {
            switch (requestType)
            {
                case RequestType.AppendRequest:
                    return HandleAppendMessages;

                case RequestType.FetchRequest:
                    return HandleLoadMessages;

                case RequestType.BrokerInfoRequest:
                    return HandleBrokerInfo;
            }

            return null;
        }

        public Response HandleAppendMessages(BinaryStream buffer)
        {
            var request = AppendRequest.ReadFromStream(buffer);

            if (!_storage.ValidatePartitionNumber(request.Topic, request.Partition))
                return null;

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

            return null;
        }

        public Response HandleLoadMessages(BinaryStream buffer)
        {
            var request = FetchRequest.ReadFromStream(buffer);

            if (!_storage.ValidatePartitionNumber(request.Topic, request.Partition))
                return null;

            var block = _storage.ReadMessagesBlock(request.Topic, request.Partition, request.Offset, request.BlockSize);

            var response = new AvailableMessagesResponse();
            response.Data = (block.Length == 0) ? new byte[0] : block.Data;

            return response;
        }

        public Response HandleBrokerInfo(BinaryStream stream)
        {
            // Not used for now
            var request = BrokerInfoRequest.ReadFromStream(stream);

            var response = new BrokerInfoResponse();
            response.HostName = _configuration.HostName;
            response.BrokerId = _configuration.BrokerId;
            response.PullPort = _configuration.PullPort;
            response.NumberOfPartitions = _configuration.NumberOfPartitions;
            response.NumberOfPartitionsPerTopic = _configuration.NumberOfPartitionsPerTopic;

            return response;
        }
    }
}