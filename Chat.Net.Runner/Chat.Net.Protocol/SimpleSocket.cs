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
        private readonly IMessageLogger _logger;

        public SimpleSocket(Socket data, IMessageLogger logger)
        {
            _data = data;
            _logger = logger;
            _data.ReceiveTimeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
        }

        private readonly byte[] buffer = new byte[1024];
        private T Receive<T>() where T : Message
        {
            try
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
                var message = JsonConvert.DeserializeObject<T>(rawMessage);
                _logger.MessageRecieved(message);
                return message;
            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.TimedOut)
                {
                    return null;
                }
                if (se.SocketErrorCode == SocketError.ConnectionReset)
                {
                    _data.Disconnect(false);
                    _data.Dispose();
                }
                throw se;
            }
        }

        public Message Receive()
        {
            return Receive<Message>();
        }

        public Message ReceiveRequired(MessageType requiredMessage, int retryCount = 1)
        {
            var count = -1;
            Message message = null;
            do
            {
                message = Receive();
                count++;
            } while (message == null || count >= retryCount);

            if (message == null)
            {
                throw new MissingMessageProtocolViolationException(requiredMessage);
            }
            if (message.Type.Name != requiredMessage.Name)
            {
                throw new SimpleProtocolViolationException(requiredMessage, message.Type);
            }

            return message;
        }

        public void Send(Message message)
        {
            var rawMessage = JsonConvert.SerializeObject(message);
            var bytes = GetBytes(rawMessage);
            _data.Send(bytes);
            _logger.MessageSent(message);
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
