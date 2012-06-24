using System;
using ZMQ;

namespace Brod.Consumers
{
    public class ConsumerContext
    {
        private readonly ZMQ.Context _zeromqContext;

        public Context ZeromqContext
        {
            get { return _zeromqContext; }
        }

        public ConsumerContext()
        {
            _zeromqContext = new ZMQ.Context(1);
        }

        public PartitionConsumer CreatePartitionConsumer(String brokerAddress)
        {
            return new PartitionConsumer("tcp://" + brokerAddress, _zeromqContext);
        }

        public PartitionConsumer CreatePartitionConsumer(ConsumerConfiguration consumerConfiguration)
        {
            consumerConfiguration.Address = "tcp://" + consumerConfiguration.Address;
            return new PartitionConsumer(consumerConfiguration, _zeromqContext);
        }

        public Consumer CreateConsumer(String brokerAddress)
        {
            return new Consumer("tcp://" + brokerAddress, this);
        }
    }
}