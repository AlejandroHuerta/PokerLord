using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Messages {
    class ClientMessage {
        public string messageType { get; set; }
        public string tableId { get; set; }
        public Message message = new Message();

        public class Message {
            public string action;
            public int amount;
        }
    }
}
