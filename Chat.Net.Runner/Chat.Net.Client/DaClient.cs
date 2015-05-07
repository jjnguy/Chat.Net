using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Chat.Net.Protocol;

namespace Chat.Net.Client
{
    public class DaClient
    {
        private SimpleSocket _simpleSock;

        public void Connect(string host = "localhost")
        {
            var sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(host, 54545);

            // connection handshake
            _simpleSock = new SimpleSocket(sock);

            var guidMessage = _simpleSock.Receive<Message>();

            var guid = Guid.Parse(guidMessage.Data);

            Console.WriteLine("Im - " + guid);

            Console.WriteLine("Enter room name:");

            _simpleSock.Send(new Message
            {
                Data = Console.ReadLine(),
                Type = MessageType.RoomRequest,
            });

            var roomAck = _simpleSock.Receive<Message>();
            if (roomAck.Type.Name != MessageType.RoomJoined.Name)
            {
                throw new ProtocolViolationException("Expected room join ack. Received: " + roomAck.Type.Name);
            }

            // handshake complete

            Task.Run(() =>
            {
                while (true)
                {
                    var message = _simpleSock.Receive<Message>();
                    if (roomAck.Type.Name != MessageType.RoomJoined.Name)
                    {
                        throw new ProtocolViolationException("Expected message. Received: " + message.Type.Name);
                    }
                    Console.Write(message.Data);
                }
            });
        }

        public void Send(Message message)
        {
            _simpleSock.Send(message);
        }
    }
}
