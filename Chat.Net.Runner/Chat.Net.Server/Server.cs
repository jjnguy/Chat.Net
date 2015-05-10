using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Chat.Net.Client;
using Chat.Net.Protocol;

namespace Chat.Net.Server
{
    public class BaseServer
    {
        private readonly IMessageLogger _logger;
        private Socket serverSock = new Socket(SocketType.Stream, ProtocolType.Tcp);

        private const string WaitingRoom = "__Waiting_Room__";

        private Dictionary<string, ChatRoom> _rooms = new Dictionary<string, ChatRoom>();

        public BaseServer(IMessageLogger logger)
        {
            _logger = logger;
            _rooms[WaitingRoom] = new ChatRoom();

            serverSock.Bind(new IPEndPoint(IPAddress.Any, 54545));
            serverSock.Listen(100);
        }

        public void Run()
        {
            while (true)
            {
                var newConnection = new SimpleSocket(serverSock.Accept(), _logger);
                Task.Run(() =>
                {
                    var guid = Guid.NewGuid();
                    Console.WriteLine("Recieved connection - " + guid);

                    newConnection.Send(new Message
                    {
                        Data = guid.ToString(),
                        Type = MessageType.ConnectionRecieved,
                    });

                    Message roomRequestMessage = null;
                    while (roomRequestMessage == null)
                    {
                        roomRequestMessage = newConnection.Receive<Message>();
                    }
                    if (roomRequestMessage.Type.Name != MessageType.RoomJoinRequest.Name)
                    {
                        throw new ProtocolViolationException("Expected room request, recieved: " + roomRequestMessage.Type.Name);
                    }
                    if (!_rooms.ContainsKey(roomRequestMessage.Data))
                    {
                        _rooms[roomRequestMessage.Data] = new ChatRoom();
                    }

                    _rooms[roomRequestMessage.Data].Join(guid, newConnection);

                    newConnection.Send(new Message
                    {
                        Data = roomRequestMessage.Data,
                        Type = MessageType.RoomJoined,
                    });
                });
            }
        }
    }

    public class ChatRoom
    {
        private readonly Dictionary<Guid, RoomMember> _connections = new Dictionary<Guid, RoomMember>();

        public void Join(Guid id, SimpleSocket newConnection)
        {
            var canceler = new CancellationTokenSource();
            var token = canceler.Token;
            _connections[id] = new RoomMember
            {
                Connection = newConnection,
                MessageListener = Task.Run(() =>
                {
                    while (true)
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        var message = newConnection.Receive();
                        if (message == null) continue;

                        if (message.Type.Name == MessageType.RoomLeaveRequest.Name)
                        {
                            Leave(message.Client.Value);
                            break;
                        }

                        if (message.Type.Name != MessageType.Message.Name)
                        {
                            throw new ProtocolViolationException("Expected a message, but recieved message of type: " + message.Type);
                        }

                        foreach (var socket in _connections.Where(con => con.Key != id))
                        {
                            socket.Value.Connection.Send(message);
                        }
                    }
                }, token),
                Cts = canceler,
            };
        }

        public void Leave(Guid id)
        {
            Console.WriteLine("Revieved request to leave room: " + id);
            var toRemove = _connections[id];
            _connections.Remove(id);
            toRemove.Cts.Cancel();
            toRemove.MessageListener.Wait(TimeSpan.FromSeconds(15));
            toRemove.MessageListener.Dispose();
        }
    }

    public class RoomMember
    {
        public SimpleSocket Connection;
        public Task MessageListener;
        public CancellationTokenSource Cts;
    }
}
