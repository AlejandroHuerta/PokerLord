using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Messages {
    

    public class EliminatedMessage {
        public string messageType { get; set; }
        public string tableId { get; set; }
        public int sequenceNumber { get; set; }
        public EliminatedInfo message { get; set; }

        public class EliminatedInfo {
            public string action { get; set; }
            public int finishingRank { get; set; }
            public string tournamentId { get; set; }
        }
    }
}
