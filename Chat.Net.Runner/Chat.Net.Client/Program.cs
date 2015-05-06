using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Net.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client!");
            var sock = new Socket(SocketType.Stream, ProtocolType.Tcp);

            sock.Connect("localhost", 54545);

            var buffer_ = new byte[1024];
            sock.Receive(buffer_);
            var guidText = GetString(buffer_);

            var guid = Guid.Parse(guidText);

            Console.WriteLine("Im - " + guid);

            Task.Run(() =>
            {
                while (true)
                {
                    var buffer = new byte[1024];
                    sock.Receive(buffer);

                    var text = GetString(buffer);
                    Console.Write(text);
                }
            });

            while (true)
            {
                var key = ConvertKeyCharToString(Console.ReadKey(true).KeyChar);
                Console.Write(key);
                sock.Send(GetBytes(key));
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
