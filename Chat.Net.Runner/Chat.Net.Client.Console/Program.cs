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

            var client = new DaClient();

            client.Connect();

            while (true)
            {
                var key = ConvertKeyCharToString(Console.ReadKey(true).KeyChar);
                Console.Write(key);
                client.Send(new Message
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
    }
}
