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
        private static List<Message> history = new List<Message>();

        static void Main(string[] args)
        {
            Console.WriteLine("Client!");

            var client = new DaClient(new ConsoleMessageLogger());

            client.Connect("anna", message =>
            {
                history.Add(message);
                RefreshConsole();
            });

            RefreshConsole();

            while (true)
            {
                var newMessage = Console.ReadLine();
                client.Send(new Message
                {
                    Type = MessageType.Message,
                    Data = newMessage,
                    Client = client.Id,
                });
            }
        }

        static void RefreshConsole()
        {
            Console.Clear();
            foreach (var message in history)
            {
                Console.WriteLine("--------------------");
                Console.WriteLine("> " + message.Data);
            }
            Console.WriteLine("===================");
        }
    }
}
