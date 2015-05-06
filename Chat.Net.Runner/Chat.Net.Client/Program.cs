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

            Task.Run(() =>
            {
                while (true)
                {
                    var buffer = new byte[1024];
                    sock.Receive(buffer);

                    var text = GetString(buffer);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        Console.WriteLine(text.Trim().Replace(((char)0) + "", ""));
                    }
                }
            });

            while (true)
            {
                var text = Console.ReadLine();
                sock.Send(GetBytes(text.Trim()));
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
            return new string(chars);
        }
    }
}
