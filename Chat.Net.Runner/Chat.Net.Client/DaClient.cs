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
        public string Username;

        public void Connect(string username, string roomName, Action<Message> messageHandler, string host = "localhost")
        {
            Username = username;
            var sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(host, 54545);

            // connection handshake
            _simpleSock = new SimpleSocket(sock, _logger);

            var guidMessage = _simpleSock.Receive();
            Id = Guid.Parse(guidMessage.Data);

            _simpleSock.Send(new Message
            {
                Data = roomName,
                Type = MessageType.RoomJoinRequest,
            });

            var roomAck = _simpleSock.Receive();
            if (roomAck.Type.Name != MessageType.RoomJoined.Name)
            {
                throw new SimpleProtocolViolationException(MessageType.RoomJoined, roomAck.Type);
            }

            // handshake complete

            Task.Run(() =>
            {
                while (true)
                {
                    var message = _simpleSock.Receive();
                    if (message == null) continue;
                    messageHandler(message);
                }
            });
        }

        public void Send(Message message)
        {
            _simpleSock.Send(new Message
            {
                Client = Id,
                ClientName = Username,
                Data = message.Data,
                Type = message.Type,
            });
        }
    }
}
