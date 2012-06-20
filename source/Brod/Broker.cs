using System;
using System.Threading;
using Brod.Tasks;

namespace Brod
{
    public class Broker
    {
        private readonly BrokerConfiguration _configuration;

        public Broker(BrokerConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Start()
        {
            var engine = new Host(
                new RequestHandlerTask(String.Format("tcp://*:{0}", _configuration.ProducerPort)),
                new HistoryHandlerTask(String.Format("tcp://*:{0}", _configuration.ConsumerPort))
            );

            using (var token = new CancellationTokenSource())
            {
                var task1 = engine.Start(token.Token, Timeout.Infinite);

                if (task1.Wait(Timeout.Infinite))
                    Console.WriteLine("Done without forced cancelation"); // This line shouldn't be reached
                else
                    Console.WriteLine("\r\nRequesting to cancel...");

                token.Cancel();
            }

        }
    }
}