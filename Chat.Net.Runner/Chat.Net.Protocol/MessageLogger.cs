using System;

namespace Chat.Net.Protocol
{
    public interface IMessageLogger
    {
        void MessageRecieved(Message m);
        void MessageSent(Message m);
        void LogException(Exception e);
    }
}
