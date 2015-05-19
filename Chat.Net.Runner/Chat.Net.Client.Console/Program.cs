using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Chat.Net.Client.Wpf;
using Chat.Net.Protocol;

namespace Chat.Net.Client
{
    class Program
    {
        private static List<Message> history = new List<Message>();

        static void Main(string[] args)
        {
            Console.SetWindowSize(120, 60);

            var client = new DaClient(new NoOpLogger());

            client.Connect("Console", "anna", message =>
            {
                history.Add(message);
                RefreshConsole();
            });

            RefreshConsole();

            while (true)
            {
                var newMessage = Console.ReadLine();
                var message = new Message
                {
                    Type = MessageType.Message,
                    Data = newMessage,
                    Client = client.Id,
                    ClientName = "me",
                };
                client.Send(message);
                history.Add(message);
                RefreshConsole();
            }
        }

        static void RefreshConsole()
        {
            Console.Clear();
            foreach (var message in history)
            {
                Console.WriteLine("--------------------");
                Console.WriteLine(message.ClientName + " > " + message.Data);
            }
            Console.WriteLine("===================");
            Console.Write("me > ");
        }
    }
}
