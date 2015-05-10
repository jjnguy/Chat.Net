using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat.Net.Protocol;

namespace Chat.Net.Client
{
    public interface IMessageLogger
    {
        void MessageRecieved(Message m);
        void MessageSent(Message m);
        void LogException(Exception e);
    }
}
