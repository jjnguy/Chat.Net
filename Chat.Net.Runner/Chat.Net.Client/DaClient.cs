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
        private IMessageLogger _logger;

        public DaClient(IMessageLogger logger)
        {
            _logger = logger;
        }

        private SimpleSocket _simpleSock;

        public Guid? Id;

        public void Connect(string roomName, Action<Message> messageHandler, string host = "localhost")
        {
            var sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(host, 54545);

            // connection handshake
            _simpleSock = new SimpleSocket(sock, _logger);

            var guidMessage = _simpleSock.Receive<Message>();
            Id = Guid.Parse(guidMessage.Data);

            _simpleSock.Send(new Message
            {
                Data = roomName,
                Type = MessageType.RoomJoinRequest,
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
                    if (message == null) continue;
                    messageHandler(message);
                }
            });
        }

        public void Send(Message message)
        {
            _simpleSock.Send(message);
        }
    }
}
