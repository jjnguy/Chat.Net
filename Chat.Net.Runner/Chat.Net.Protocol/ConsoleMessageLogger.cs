using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat.Net.Client;

namespace Chat.Net.Protocol
{
    public class ConsoleMessageLogger : IMessageLogger
    {
        public void MessageRecieved(Message m)
        {
            Console.WriteLine("Recieved message of type: " + m.Type.Name + ", with data: " + m.Data);
        }

        public void MessageSent(Message m)
        {
            Console.WriteLine("Sent message of type: " + m.Type.Name + ", with data: " + m.Data);
        }

        public void LogException(Exception e)
        {
            Console.WriteLine("Error occurred: " + e.Message);
        }
    }
}
