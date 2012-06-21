using System;
using System.Collections.Generic;
using System.Linq;
using Brod.Exceptions;
using Brod.Tests.Specs.StorageArea.contexts;
using Machine.Specifications;

namespace Brod.Tests.Specs.StorageArea
{
    public class when_reading_using_offset_of_one_byte : storage_with_sample_data
    {
        Because of = () =>
        {
            var bytesToRead = Message.CalculateMessageSize(messageBytes.Length) * 4 - 1;

            exception = Catch.Exception(() =>
                messages = storage
                    .ReadMessages("test", 0, 1, bytesToRead)
                    .ToList()
            );

        };

        /// <summary>
        /// This is not deterministic test, because in some conditions it
        /// may throw CorruptedMessageException.
        /// </summary>
        It should_has_zero_messages = () =>
            messages.Count.ShouldEqual(0);

        public static List<Message> messages;
        private static Exception exception;
    }
}