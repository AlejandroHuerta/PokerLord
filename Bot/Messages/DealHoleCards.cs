using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Messages {
    class DealHoleCards {
        public string messageType { get; set; }
        public string tableId { get; set; }
        public int sequenceNumber { get; set; }
        public Message message { get; set; }

        public class Message {
            public List<List<string>> cards { get; set; }
        }
    }
}
