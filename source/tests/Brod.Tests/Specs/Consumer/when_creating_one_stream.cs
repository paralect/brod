using System.Collections.Generic;
using Brod.Consumers;
using Machine.Specifications;

namespace Brod.Tests.Specs.Consumer
{
    public class when_creating_one_stream : _consumer_connector_context
    {
        Because of = () =>
        {
            streams = connector.CreateMessageStreams(new Dictionary<string, int>() {{"test", 1 }});
        };

        It should_has_one_entry = () =>
            streams.Count.ShouldEqual(1);

        It should_has_one_stream_for_topic_test = () =>
            streams["test"].Count.ShouldEqual(1);

        It should_has_correct_topic_name = () =>
            streams["test"][0].Topic.ShouldEqual("test");

        It should_has_correct_number_of_partitions = () =>
            streams["test"][0].Partitions.Count.ShouldEqual(1);

        It should_has_correct_value_of_partition = () =>
            streams["test"][0].Partitions[0].ShouldEqual(0);

        private static Dictionary<string, List<MessageStream>> streams;
    }
}