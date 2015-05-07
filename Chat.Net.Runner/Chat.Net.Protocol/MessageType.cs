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
        public static readonly MessageType RoomRequest = new MessageType("room-request");
        public static readonly MessageType RoomJoined = new MessageType("room-joined");
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

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(obj, this)) return true;

            var casted = obj as MessageType;

            if (casted == null) return false;

            return casted.Name == this.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
