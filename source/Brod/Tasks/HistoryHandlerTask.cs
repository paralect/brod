using System;
using System.IO;
using System.Text;
using System.Threading;
using Brod.Sockets;

namespace Brod.Tasks
{
    public class HistoryHandlerTask : ITask
    {
        private readonly BrokerConfiguration _configuration;
        private readonly string _repAddress;
        private ZMQ.Context _zeromqContext;

        public HistoryHandlerTask(BrokerConfiguration configuration)
        {
            _configuration = configuration;
            _repAddress = String.Format("tcp://*:{0}", configuration.ConsumerPort);
        }

        public void Run(CancellationToken token)
        {
            using (Socket repSocket = CreateSocket(ZMQ.SocketType.REP))
            {
                // Bind to socket
                repSocket.Bind(_repAddress);

                // Process while canellation not requested
                while (!token.IsCancellationRequested)
                {
                    // Waits for messages
                    var data = repSocket.Recv(200);
                    if (data == null) continue;

                    var result = Encoding.UTF8.GetString(data);
                    result = "Answer:" + result;

                    repSocket.Send(Encoding.UTF8.GetBytes(result));
                }
            }            
        }

        public void Init()
        {
            _zeromqContext = new ZMQ.Context(1);
        }

        public void Dispose()
        {
            if (_zeromqContext != null)
                _zeromqContext.Dispose();
        }

        public Socket CreateSocket(ZMQ.SocketType socketType)
        {
            var zmqsocket = _zeromqContext.Socket(socketType);
            var socket = new Socket(zmqsocket);
            return socket;
        }
    }
}