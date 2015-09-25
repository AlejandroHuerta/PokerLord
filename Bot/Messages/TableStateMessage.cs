using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Messages {
    class TableStateMessage {
        public string messageType { get; set; }
        public string tableId { get; set; }
        public int sequenceNumber { get; set; }
        public StateMessage message { get; set; }

        public class StateMessage {
            public string action { get; set; }
            public int handNumber { get; set; }
            public string tableName { get; set; }
            public int tableSize { get; set; }
            public string tournamentId { get; set; }
            public string tournamentName { get; set; }
            public string stakesLevelName { get; set; }
            public string stakesLevelType { get; set; }
            public string nextStakesLevelName { get; set; }
            public string nextStakesLevelType { get; set; }
            public int currentStakesLevelTimeLeft { get; set; }
            public int dealerSeatNumber { get; set; }
            public ActivePlayer activePlayer { get; set; }
            public List<StatePlayer> players { get; set; }
            public GameParameters gameParameters { get; set; }
            public List<string> tableCards { get; set; }
            public List<List<string>> holeCards { get; set; }
            public string externalTournamentId { get; set; }

            public class ActivePlayer {
                public int? seatNumber { get; set; }
                public Timeout timeout { get; set; }

                public class Timeout {
                    public string now { get; set; }
                    public string started { get; set; }
                    public string ends { get; set; }
                }
            }
        }

        public class GameParameters {
            public string id { get; set; }
            public string format { get; set; }
            public int bigBlind { get; set; }
            public int smallBlind { get; set; }
            public int minimumBet { get; set; }
            public int callAmount { get; set; }
            public List<int?> contributedThisRound { get; set; }
            public int currentGameStakesLevel { get; set; }
            public List<int?> pots { get; set; }
            public List<object> bets { get; set; }
        }
    }
}
