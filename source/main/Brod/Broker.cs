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
            using(var store = new Store.Store(_configuration))
            {
                var host = new Host(
                    new RequestHandlerTask(_configuration, store),
                    new HistoryHandlerTask(_configuration, store),
                    new FlusherTask(_configuration, store)
                );

                using (var token = new CancellationTokenSource())
                using (host)
                {
                    var task1 = host.Start(token.Token, Timeout.Infinite);

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