using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Net.Server
{
    public class Connection
    {
        public readonly Guid Id;
        public readonly string Handle;
        private readonly Socket _socket;

        public Connection(string handle, Socket socket, Action<string> messageHansler)
        {
            Handle = handle;
            _socket = socket;
            Id = Guid.NewGuid();

            Task.Run(() =>
            {
                byte[] buffer = new byte[1024];
                _socket.Receive(buffer);
                var text = GetString(buffer);
                messageHansler(text);
            }).Start();
        }

        public void Send(string message)
        {
            _socket.Send(GetBytes(message));
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
            return new string(chars);
        }
    }
}
