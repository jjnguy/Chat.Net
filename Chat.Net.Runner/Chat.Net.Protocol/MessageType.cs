using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Net.Protocol
{
    public class MessageType
    {
        public static readonly MessageType ConnectionRecieved = new MessageType("connection-recieved");
        public static readonly MessageType RoomJoinRequest = new MessageType("room-join-request");
        public static readonly MessageType RoomLeaveRequest = new MessageType("room-leave-request");
        public static readonly MessageType RoomJoined = new MessageType("room-joined");
        public static readonly MessageType RoomLeft = new MessageType("room-left");
        public static readonly MessageType Message = new MessageType("message");

        public string Name;

        [Obsolete("Use for reflective instantiation only")]
        public MessageType() { }

        private MessageType(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
