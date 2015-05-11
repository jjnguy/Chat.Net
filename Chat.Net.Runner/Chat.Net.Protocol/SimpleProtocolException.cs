using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Net.Protocol
{
    /// <summary>
    /// Represents a case when and unexpected message type was recieved
    /// </summary>
    public class SimpleProtocolViolationException : ProtocolViolationException
    {
        public SimpleProtocolViolationException(MessageType expected, MessageType recieved)
            : base("Expected message of type '" + expected.Name + "', but recieved '" + recieved.Name + "'")
        {
        }
    }

    public class MissingMessageProtocolViolationException : ProtocolViolationException
    {
        public MissingMessageProtocolViolationException(MessageType expected)
            : base("Expected message of type '" + expected.Name + "', but none was recived within the ")
        {
        }
    }
}
