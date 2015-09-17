using Bot.Messages;
using LogParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot {
    class BotManager : XmppManager.MessageHandler {
        public interface BotInterface {
            string GetTableId();
            void ResetActions();
            void SetPlayers(List<StatePlayer> players);
            void ActivePlayer(int seat);
            void SetPlayerAction(int seat, Player.Action action);
            void TableCards(List<string> cards);
            void SetBalance(int seat, int balance);
            void SetMinimumBet(int minBet);
        }

        private static BotManager instance;

        public static BotManager Instance {
            get {
                if (instance == null) {
                    instance = new BotManager();
                }
                return instance;
            }//get
        }//Instance

        List<BotInterface> bots = new List<BotInterface>();
        string username = "";

        private BotManager() {

        }

        public void SetUsername(string user) {
            username = user;
        }

        public void OpenTable(OpenTableMessage message) {
            bots.Add(new Bot(message.tableId, username));
        }

        public void TableState(TableStateMessage message) {
            var bot = bots.Find(b => { return b.GetTableId() == message.tableId; });
            if (bot == null) {
                bot = new Bot(message.tableId, username);
                bots.Add(bot);
            }
            bot.SetPlayers(message.message.players);
        }

        public void BeginGame(BeginGame message) {
            GetBot(message.tableId)?.ResetActions();
        }//BeginGame

        public void SetPlayers(SetPlayers message) {
            GetBot(message.tableId)?.SetPlayers(message.message.players);
        }

        public void SetActivePlayer(SetActivePlayer message) {
            GetBot(message.tableId)?.ActivePlayer(message.message.seatNumber);
        }

        public void SetPlayerAction(SetPlayerAction message) {
            Player.Action action = Player.Action.Out;

            switch (message.message.actionName) {
            case "CALL":
                action = Player.Action.Call;
                break;
            case "FOLD":
                action = Player.Action.Fold;
                break;
            case "BET":
                action = Player.Action.Bet;
                break;
            case "CHECK":
                action = Player.Action.Check;
                break;
            case "ALLIN":
                action = Player.Action.AllIn;
                break;
            case "RAISE":
                action = Player.Action.Raise;
                break;
            case "POST":
                action = Player.Action.Posted;
                break;
            default:
                Console.WriteLine("Action {0} is not being handled", message.message.actionName);
                break;
            }//switch

            var bot = bots.Find(b => { return b.GetTableId() == message.tableId; });
            bot?.SetPlayerAction(message.message.seatNumber, action);
            bot?.SetBalance(message.message.seatNumber, message.message.balance);
            bot?.SetMinimumBet(message.message.minimumBet);
        }

        public void Eliminated(EliminatedMessage message) {
            bots.Remove(bots.Find(bot => { return bot.GetTableId() == message.tableId; }));
            Console.WriteLine("Bot at table {0} finished in {1} place", message.tableId, message.message.finishingRank);
        }//Eliminated

        public void DealCommunityCards(DealCommunityCards message) {
            bots.Find(bot => { return bot.GetTableId() == message.tableId; })?.TableCards(message.message.cards);
        }

        BotInterface GetBot(string tableId) {
            var bot = bots.Find(b => { return b.GetTableId() == tableId; });
            if (bot == null) {
                XmppManager.Instance.RequestState(tableId);
            }

            return bot;
        }
    }//BotManager
}
