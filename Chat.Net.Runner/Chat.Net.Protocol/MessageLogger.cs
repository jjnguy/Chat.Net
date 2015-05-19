using System;

namespace Chat.Net.Protocol
{
    public interface IMessageLogger
    {
        void MessageRecieved(Message m);
        void MessageSent(Message m);
        void LogException(Exception e);
    }


    public class NoOpLogger : IMessageLogger
    {
        public void MessageRecieved(Message m)
        {
        }

        public void MessageSent(Message m)
        {
        }

        public void LogException(Exception e)
        {
        }
    }
}
