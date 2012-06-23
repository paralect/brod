using System;
using System.IO;
using System.Text;
using System.Threading;
using Brod.Requests;
using Brod.Sockets;

namespace Brod.Producers
{
    public class Producer : IDisposable
    {
        private readonly string _address;
        private readonly ZMQ.Context _zeromqContext;
        private readonly Socket _pushSocket;

        public Producer(String address, ZMQ.Context zeromqContext)
        {
            _address = address;
            _zeromqContext = zeromqContext;
            _pushSocket = CreateSocket(ZMQ.SocketType.PUSH);

            // Bind to socket
            _pushSocket.Connect(_address, CancellationToken.None);                
        }

        /// <summary>
        /// Send message with specified encoding
        /// </summary>
        public void Send(String topic, Int32 partition, String message, Encoding encoding)
        {
            Send(topic, partition, encoding.GetBytes(message));
        }

        /// <summary>
        /// Send message with default UTF-8 encoding
        /// </summary>
        public void Send(String topic, Int32 partition, String message)
        {
            Send(topic, partition, message, Encoding.UTF8);
        }

        /// <summary>
        /// Send binary message
        /// </summary>
        public void Send(String topic, Int32 partition, byte[] payload)
        {
            var request = new AppendMessagesRequest(topic, partition, Message.CreateMessage(payload));

            using(var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                request.WriteToStream(stream, writer);

                var data = stream.ToArray();
                //Console.WriteLine("Sending {0} bytes", data.Length);
                _pushSocket.Send(data);
            }
        }

        private Socket CreateSocket(ZMQ.SocketType socketType)
        {
            var zmqsocket = _zeromqContext.Socket(socketType);
            var socket = new Socket(zmqsocket);
            return socket;
        }

        public void Dispose()
        {
            if (_pushSocket != null)
                _pushSocket.Dispose();
        }
    }
}