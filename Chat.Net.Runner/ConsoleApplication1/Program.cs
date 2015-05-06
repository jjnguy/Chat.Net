using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new Socket(SocketType.Stream, ProtocolType.Tcp);

            connection.Connect("localhost", 54545);

            var buffer = new byte[10];

            connection.Receive(buffer);

            var data = GetString(buffer);
            Console.WriteLine(data);
            Console.Read();
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}
