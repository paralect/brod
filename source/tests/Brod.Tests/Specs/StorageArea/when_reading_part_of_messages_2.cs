using System.Collections.Generic;
using System.Linq;
using Brod.Messages;
using Brod.Tests.Specs.StorageArea.contexts;
using Machine.Specifications;

namespace Brod.Tests.Specs.StorageArea
{
    public class when_reading_part_of_messages_2 : storage_with_sample_data
    {
        Because of = () =>
        {
            var bytesToRead = Message.CalculateOnDiskMessageLength(messageBytes.Length) * 4 + 1;

            messages = storage
                .ReadMessages("test", 0, 0,  bytesToRead)
                .ToList();
        };

        It should_has_three_messages = () =>
            messages.Count.ShouldEqual(4);
            
        public static List<Message> messages;
    }
}