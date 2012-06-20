using System;
using System.IO;
using System.Text;
using System.Threading;
using Brod.Sockets;

namespace Brod.Tasks
{
    public class RequestHandlerTask : ITask
    {
        private readonly string _pullAddress;
        private ZMQ.Context _zeromqContext;

        public RequestHandlerTask(String pullAddress)
        {
            _pullAddress = pullAddress;
        }

        public void Run(CancellationToken token)
        {
            using (Socket processPullSocket = CreateSocket(ZMQ.SocketType.PULL))
            {
                // Bind to socket
                processPullSocket.Bind(_pullAddress);

                // Process while canellation not requested
                while (!token.IsCancellationRequested)
                {
                    // Waits for messages
                    var data = processPullSocket.Recv(200);
                    if (data == null) continue;

                    using (var reader = new MessageReader(new MemoryStream(data)))
                    {
                        Message message;
                        while ((message = reader.ReadMessage()) != null)
                        {
                            Console.WriteLine(Encoding.UTF8.GetString(message.Payload));
                        }
                    }
                }
            }            
        }

        public void Init()
        {
            _zeromqContext = new ZMQ.Context(2);
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