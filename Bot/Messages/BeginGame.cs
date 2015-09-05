using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Messages {
    class BeginGame {
        public string messageType { get; set; }
        public string tableId { get; set; }
        public int sequenceNumber { get; set; }
        public Message message { get; set; }

        public class Message {
            public string action { get; set; }
            public string id { get; set; }
            public string format { get; set; }
            public int bigBlind { get; set; }
            public int smallBlind { get; set; }
            public int stakesLevel { get; set; }
            public List<int> inPlay { get; set; }
            public string stakesLevelName { get; set; }
            public string stakesLevelType { get; set; }
            public string nextStakesLevelName { get; set; }
            public string nextStakesLevelType { get; set; }
            public int currentStakesLevelTimeLeft { get; set; }
            public int? handNumber { get; set; }
        }
    }
}
