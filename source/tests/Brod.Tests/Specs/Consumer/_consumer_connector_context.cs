using Brod.Consumers;
using Machine.Specifications;

namespace Brod.Tests.Specs.Consumer
{
    public class _consumer_connector_context
    {
        Establish context = () =>
        {
            configuration = new ConsumerConfiguration();
            connector = new ConsumerConnector(configuration, new ZMQ.Context());
        };

        public static ConsumerConfiguration configuration;
        public static ConsumerConnector connector;
    }
}