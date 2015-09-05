using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Messages {
    public class OpenTableMessage {
        public string messageType { get; set; }
        public string tableId { get; set; }
        public int sequenceNumber { get; set; }
        public GameMessage message { get; set; }

        public class GameMessage {
            public string action { get; set; }
            public string externalTournamentId { get; set; }
            public string tableName { get; set; }
            public int tableSize { get; set; }
            public string tournamentName { get; set; }
        }//GameMessage
    }//OpenTableMessage
}//Bot.Messages
