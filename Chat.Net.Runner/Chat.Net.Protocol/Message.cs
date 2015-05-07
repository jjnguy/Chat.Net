using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Net.Protocol
{
    public class Message
    {
        public MessageType Type { get; set; }

        public string Data { get; set; }
    }
}
