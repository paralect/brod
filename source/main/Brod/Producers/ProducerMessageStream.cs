using System;
using System.IO;
using System.Text;
using System.Threading;
using Brod.Requests;
using Brod.Sockets;

namespace Brod.Producers
{
    public class ProducerMessageStream : IDisposable
    {
        private readonly string _address;
        private readonly string _topic;
        private readonly ZMQ.Context _zeromqContext;
        private readonly Socket _pushSocket;
        private readonly Int32 _partition;

        public ProducerMessageStream(String address, String topic, ZMQ.Context zeromqContext)
        {
            _address = address;
            _topic = topic;
            _partition = 0;
            _zeromqContext = zeromqContext;

            _pushSocket = CreateSocket(ZMQ.SocketType.PUSH);
            _pushSocket.Connect(_address, CancellationToken.None);
        }

        /// <summary>
        /// Send message with specified encoding
        /// </summary>
        public void Send(String message, Encoding encoding)
        {
            Send(message, encoding, _partition);
        }

        /// <summary>
        /// Send message with specified encoding to specified partition
        /// </summary>
        public void Send(String message, Encoding encoding, Int32 partition)
        {
            Send(encoding.GetBytes(message), partition);
        }

        /// <summary>
        /// Send message with default UTF-8 encoding
        /// </summary>
        public void Send(String message)
        {
            Send(message, _partition);
        }


        /// <summary>
        /// Send message with default UTF-8 encoding to specified partition
        /// </summary>
        public void Send(String message, Int32 partition)
        {
            Send(message, Encoding.UTF8, partition);
        }

        /// <summary>
        /// Send binary message
        /// </summary>
        public void Send(byte[] payload)
        {
            Send(payload, _partition);
       }

        /// <summary>
        /// Send binary message to specified topic partition
        /// </summary>
        public void Send(byte[] payload, Int32 partition)
        {
            var request = new AppendMessagesRequest(_topic, _partition, Message.CreateMessage(payload));

            using (var stream = new MemoryStream())
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