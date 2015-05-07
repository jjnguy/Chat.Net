using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chat.Net.Protocol
{
    public class SimpleSocket
    {
        private readonly Socket _data;

        public SimpleSocket(Socket data)
        {
            _data = data;
        }

        private readonly byte[] buffer = new byte[1024];
        public Message Receive()
        {
            var rawMessage = "";
            var bytesRead = 0;
            while (true)
            {
                bytesRead = _data.Receive(buffer);
                if (bytesRead == 0) break;
                rawMessage += GetString(buffer, bytesRead);
                if (bytesRead != buffer.Length) break;
            }

            return JsonConvert.DeserializeObject<Message>(rawMessage);
        }

        public void Send(Message message)
        {
            var rawMessage = JsonConvert.SerializeObject(message);
            var bytes = GetBytes(rawMessage);
            _data.Send(bytes);
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes, int bytesRead)
        {
            char[] chars = new char[bytesRead / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytesRead);
            return new string(chars).Replace(((char)0).ToString(), "");
        }
    }
}
