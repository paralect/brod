using System;
using System.Threading;
using Brod.Common.Tasks;
using Brod.Network;
using Brod.Storage;

namespace Brod.Brokers
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
            using(var store = new Store(_configuration))
            {
                var handlers = new RequestHandlers(_configuration, store);

                var engine = new TaskEngine(
                    new SocketListener(ZMQ.SocketType.REP, _configuration.Port, handlers.MapHandlers),
                    new SocketListener(ZMQ.SocketType.PULL, _configuration.PullPort, handlers.MapHandlers),
                    new Flusher(_configuration, store));

                using (var token = new CancellationTokenSource())
                using (engine)
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
}