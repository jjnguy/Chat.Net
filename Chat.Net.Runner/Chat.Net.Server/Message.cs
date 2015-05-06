using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Net.Server
{
    public class Message
    {
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid Sender { get; set; }
        public string SenderName { get; set; }
    }
}
