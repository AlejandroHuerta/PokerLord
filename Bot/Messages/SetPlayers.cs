using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Messages {
    class SetPlayers {
        public string messageType { get; set; }
        public string tableId { get; set; }
        public int sequenceNumber { get; set; }
        public Message message { get; set; }        

        public class Dealer {
            public int seatNumber { get; set; }
        }

        public class ActivePlayerTimeout {
            public string now { get; set; }
            public object started { get; set; }
            public object ends { get; set; }
        }

        public class Message {
            public string action { get; set; }
            public List<StatePlayer> players { get; set; }
            public Dealer dealer { get; set; }
            public object activeSeatNumber { get; set; }
            public ActivePlayerTimeout activePlayerTimeout { get; set; }
        }
    }
}
