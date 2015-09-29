using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Messages {
    class SetActivePlayer {
        public string messageType { get; set; }
        public string tableId { get; set; }
        public int sequenceNumber { get; set; }
        public Message message { get; set; }        

        public class Message {
            public string action { get; set; }
            public int seatNumber { get; set; }
            public bool showMuck { get; set; }
            public int? pot { get; set; }
            public int callAmount { get; set; }
            public List<int?> contributedThisRound { get; set; }
            public int minimumBet { get; set; }
            public Timeout timeout { get; set; }
        }
    }
}
