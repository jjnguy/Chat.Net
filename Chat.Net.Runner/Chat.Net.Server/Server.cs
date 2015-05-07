using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chat.Net.Protocol;

namespace Chat.Net.Server
{
    public class BaseServer
    {
        private Socket serverSock = new Socket(SocketType.Stream, ProtocolType.Tcp);

        private Dictionary<string, ChatRoom> _rooms = new Dictionary<string, ChatRoom>();

        public BaseServer()
        {
            serverSock.Bind(new IPEndPoint(IPAddress.Any, 54545));
            serverSock.Listen(100);

            while (true)
            {
                var newConnection = new SimpleSocket(serverSock.Accept());
                var guid = Guid.NewGuid();
                Console.WriteLine("Recieved connection - " + guid);

                newConnection.Send(new Protocol.Message
                {
                    Data = guid.ToString(),
                    Type = MessageType.ConnectionRecieved,
                });

                var roomRequestMessage = newConnection.Receive();
                if (roomRequestMessage.Type.Name != MessageType.RoomRequest.Name)
                {
                    throw new ProtocolViolationException("Expected room request, recieved: " + roomRequestMessage.Type.Name);
                }
                if (!_rooms.ContainsKey(roomRequestMessage.Data))
                {
                    _rooms[roomRequestMessage.Data] = new ChatRoom();
                }

                _rooms[roomRequestMessage.Data].Join(guid, newConnection);

                newConnection.Send(new Protocol.Message
                {
                    Data = roomRequestMessage.Data,
                    Type = MessageType.RoomJoined,
                });
            }
        }
    }

    public class ChatRoom
    {
        private Dictionary<Guid, SimpleSocket> _connections = new Dictionary<Guid, SimpleSocket>();

        public void Join(Guid id, SimpleSocket newConnection)
        {
            _connections[id] = newConnection;

            Task.Run(() =>
            {
                while (true)
                {
                    var message = newConnection.Receive();
                    if (message.Type.Name != MessageType.Message.Name)
                    {
                        throw new ProtocolViolationException("Expected a message, but recieved message of type: " + message.Type);
                    }
                    foreach (var socket in _connections.Where(con => con.Key != id))
                    {
                        socket.Value.Send(message);
                    }
                }
            });
        }
    }
}
