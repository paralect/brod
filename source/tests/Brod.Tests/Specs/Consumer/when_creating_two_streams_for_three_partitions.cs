using System.Collections.Generic;
using Brod.Consumers;
using Machine.Specifications;

namespace Brod.Tests.Specs.Consumer
{
    public class when_creating_two_streams_for_three_partitions : _consumer_connector_context
    {
        Establish context = () =>
        {
            configuration.NumberOfPartitions = 3;
        };

        Because of = () =>
        {
            streams = connector.CreateMessageStreams(new Dictionary<string, int>() { { "test", 2 } });
        };

        It should_has_one_entry = () =>
            streams.Count.ShouldEqual(1);

        It should_has_two_streams_for_topic_test = () =>
            streams["test"].Count.ShouldEqual(2);

        It should_has_correct_number_of_partitions = () =>
        {
            streams["test"][0].Partitions.Count.ShouldEqual(2);
            streams["test"][1].Partitions.Count.ShouldEqual(1);
        };

        It should_has_correct_value_of_partition = () =>
        {
            streams["test"][0].Partitions[0].ShouldEqual(0);
            streams["test"][0].Partitions[1].ShouldEqual(1);
            streams["test"][1].Partitions[0].ShouldEqual(2);
        };

        private static Dictionary<string, List<ConsumerMessageStream>> streams;
    }
}