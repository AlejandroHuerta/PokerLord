using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Messages {
    class SetPlayerAction {
        public string messageType { get; set; }
        public string tableId { get; set; }
        public int sequenceNumber { get; set; }
        public Message message { get; set; }

        public class Message {
            public string action { get; set; }
            public int seatNumber { get; set; }
            public string actionName { get; set; }
            public int amount { get; set; }
            public string nameplateStyle { get; set; }
            public int balance { get; set; }
            public int playerChipstackAmount { get; set; }
            public int callAmount { get; set; }
            public int? pot { get; set; }
            public List<int?> contributedThisRound { get; set; }
            public int minimumBet { get; set; }
        }
    }
}
