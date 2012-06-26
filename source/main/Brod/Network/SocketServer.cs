using System;
using System.Threading;
using System.Threading.Tasks;
using ZMQ;

namespace Brod.Network
{
    public class SocketServer
    {
        private readonly int _repPort;
        private readonly int _pullPort;
        private readonly int _numberOfThreads;

        public SocketServer(Int32 repPort, Int32 pullPort, Int32 numberOfThreads)
        {
            _repPort = repPort;
            _pullPort = pullPort;
            _numberOfThreads = numberOfThreads;
        }

        public void Startup()
        {
            var acceptor = new Acceptor(_repPort, _pullPort);
            acceptor.Run();
            //Task.Factory.StartNew(acceptor.Run);
        }

        public void Shutdown()
        {
            
        }
    }

    /// <summary>
    /// Accepter represent thread that accepts new connections. 
    /// There is only need for one of these
    /// </summary>
    public class Acceptor
    {
        private readonly int _repPort;
        private readonly int _pullPort;

        public Acceptor(int repPort, int pullPort)
        {
            _repPort = repPort;
            _pullPort = pullPort;
        }

        public void Run()
        {
            Console.WriteLine("TreadId: {0}", Thread.CurrentThread.ManagedThreadId);
            var repAddress = String.Format("tcp://*:{0}", _repPort);
            var pullAddress = String.Format("tcp://*:{0}", _pullPort);

            using (Context context = new Context(1))
            using (var repSocket = context.Socket(ZMQ.SocketType.REP))
            using (var pullSocket = context.Socket(ZMQ.SocketType.PULL))
            {
                repSocket.Bind(repAddress);
                pullSocket.Bind(pullAddress);

                PollItem repPollItem = repSocket.CreatePollItem(IOMultiPlex.POLLIN);
                repPollItem.PollInHandler += new PollHandler(repPollItem_PollInHandler);

                PollItem pullPollItem = pullSocket.CreatePollItem(IOMultiPlex.POLLIN);
                pullPollItem.PollInHandler += new PollHandler(pullPollItem_PollInHandler);

                PollItem[] pollItems = new[] { repPollItem, pullPollItem };

                while (true)
                {
                    context.Poll(pollItems);
                }                
            }
        }

        void pullPollItem_PollInHandler(Socket socket, IOMultiPlex revents)
        {
            Console.WriteLine("TreadId: {0}", Thread.CurrentThread.ManagedThreadId);
            byte[] bytesRecv = socket.Recv();
            Console.WriteLine("Pull derg!");
        }

        void repPollItem_PollInHandler(Socket socket, IOMultiPlex revents)
        {
            Console.WriteLine("TreadId: {0}", Thread.CurrentThread.ManagedThreadId);
            byte[] bytesRecv = socket.Recv();
            Console.WriteLine("Rep derg!");
        }
    }
}