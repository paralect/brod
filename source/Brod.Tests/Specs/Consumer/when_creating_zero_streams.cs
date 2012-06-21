using System;
using System.Collections.Generic;
using Brod.Consumers;
using Machine.Specifications;

namespace Brod.Tests.Specs.Consumer
{
    public class when_creating_zero_streams : _consumer_connector_context
    {
        Because of = () =>
        {
            exception = Catch.Exception(() =>
            {
                connector.CreateMessageStreams(new Dictionary<string, int>() { { "test", 0 } });
            });

        };

        It should_throw = () =>
            exception.ShouldBeOfType<Exception>();

        private static Exception exception;
    }
}