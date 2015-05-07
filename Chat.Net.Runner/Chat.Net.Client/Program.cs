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
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client!");
            var sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            sock.Connect("localhost", 54545);

            var simpleSock = new SimpleSocket(sock);

            var guidMessage = simpleSock.Receive();

            var guid = Guid.Parse(guidMessage.Data);

            Console.WriteLine("Im - " + guid);

            Console.WriteLine("Enter room name:");

            simpleSock.Send(new Message
            {
                Data = Console.ReadLine(),
                Type = MessageType.RoomRequest,
            });

            var roomAck = simpleSock.Receive();
            if (roomAck.Type.Name != MessageType.RoomJoined.Name)
            {
                throw new ProtocolViolationException("Expected room join ack. Received: " + roomAck.Type.Name);
            }

            Task.Run(() =>
            {
                while (true)
                {
                    var message = simpleSock.Receive();
                    if (roomAck.Type.Name != MessageType.RoomJoined.Name)
                    {
                        throw new ProtocolViolationException("Expected message. Received: " + message.Type.Name);
                    }
                    Console.Write(message.Data);
                }
            });

            while (true)
            {
                var key = ConvertKeyCharToString(Console.ReadKey(true).KeyChar);
                Console.Write(key);
                simpleSock.Send(new Message
                {
                    Type = MessageType.Message,
                    Data = key
                });
            }
        }

        private static string ConvertKeyCharToString(char key)
        {
            switch (key)
            {
                case '\r':
                case '\n':
                    return "\r\n";
                default:
                    return key.ToString();
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
