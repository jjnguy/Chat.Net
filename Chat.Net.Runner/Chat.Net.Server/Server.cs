using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Net.Server
{
    public class BaseServer
    {
        private Socket serverSock = new Socket(SocketType.Stream, ProtocolType.Tcp);

        private Dictionary<Guid, Socket> _connections = new Dictionary<Guid, Socket>();

        public BaseServer()
        {
            serverSock.Bind(new IPEndPoint(IPAddress.Any, 54545));
            serverSock.Listen(100);

            while (true)
            {
                var newConnection = serverSock.Accept();
                var guid = Guid.NewGuid();
                Console.WriteLine("Recieved connection - " + guid);
                newConnection.Send(GetBytes(guid.ToString()));
                _connections.Add(guid, newConnection);

                Task.Run(() =>
                {
                    while (true)
                    {
                        var buffer = new byte[1024];
                        newConnection.Receive(buffer);
                        var text = GetString(buffer);
                        foreach (var socket in _connections.Where(con => con.Key != guid))
                        {
                            socket.Value.Send(GetBytes(text));
                        }
                    }
                });
            }
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars).Replace(((char)0).ToString(), "");
        }
    }
}
