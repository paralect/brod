using System;
using System.Threading;
using Brod.Tasks;

namespace Brod
{
    public class Server
    {
        public void Start()
        {
            var engine = new Host(
                new RequestHandlerTask("tcp://*:5567"),
                new HistoryHandlerTask("tcp://*:5568")
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