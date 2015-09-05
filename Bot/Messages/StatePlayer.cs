using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Messages {
    public class StatePlayer {
        public string avatarUrl { get; set; }
        public string lastAction { get; set; }
        public int balance { get; set; }
        public string userJid { get; set; }
        public string username { get; set; }
        public string userId { get; set; }
        public int seatNumber { get; set; }
        public string nameplateStyle { get; set; }
    }
}
