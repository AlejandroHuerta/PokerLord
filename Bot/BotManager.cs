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
            List<Player> Players { get; }
            void PlayerCards(List<List<string>> cards);
            void ClearTableCards();
            void SetContribution(int seat, int amount);
            void ClearContributions();
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
            if (message.message.holeCards != null) {
                bot.PlayerCards(message.message.holeCards);
            }//if
            if (message.message.gameParameters.contributedThisRound != null) {
                SetContribution(bot, message.message.gameParameters.contributedThisRound);
            }

            if (message.message.activePlayer.seatNumber != null) {
                bot.ActivePlayer((int)message.message.activePlayer.seatNumber);
            }//if
        }//TableState

        public void BeginGame(BeginGame message) {
            GetBot(message.tableId)?.ResetActions();
            GetBot(message.tableId)?.ClearTableCards();
            GetBot(message.tableId)?.ClearContributions();
        }//BeginGame

        public void SetPlayers(SetPlayers message) {
            GetBot(message.tableId)?.SetPlayers(message.message.players);
        }

        public void SetActivePlayer(SetActivePlayer message) {
            GetBot(message.tableId)?.ActivePlayer(message.message.seatNumber);
        }

        public void SetPlayerAction(SetPlayerAction message) {
            Player.Action action = Player.Action.Out;

            var bot = GetBot(message.tableId);
            if (bot == null) {
                return;
            }//if

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
                if (bot.Players.Exists(p => p.LastAction == Player.Action.SmallBlind)) {
                    action = Player.Action.BigBlind;
                } else {
                    action = Player.Action.SmallBlind;
                }//else
                break;
            default:
                Console.WriteLine("Action {0} is not being handled", message.message.actionName);
                break;
            }//switch
            
            bot.SetPlayerAction(message.message.seatNumber, action);
            bot.SetBalance(message.message.seatNumber, message.message.balance);
            bot.SetMinimumBet(message.message.minimumBet);

            SetContribution(bot, message.message.contributedThisRound);

            Console.WriteLine(bot?.ToString());
        }//SetPlayerAction

        void SetContribution(BotInterface bot, List<int?> contribution) {
            for(int i = 0; i < contribution.Count; i++) {
                if (contribution[i] != null) {
                    bot.SetContribution(i, (int)contribution[i]);
                }//if
            }//for
        }//SetContribution

        public void Eliminated(EliminatedMessage message) {
            bots.Remove(GetBot(message.tableId));
            Console.WriteLine("Bot at table {0} finished in {1} place", message.tableId, message.message.finishingRank);
        }//Eliminated

        public void DealCommunityCards(DealCommunityCards message) {
            var bot = GetBot(message.tableId);
            bot?.TableCards(message.message.cards);
            bot?.ResetActions();
            bot?.ClearContributions();
            Console.WriteLine(bot?.ToString());
        }

        public void DealHoleCards(DealHoleCards message) {
            var bot = GetBot(message.tableId);
            bot?.PlayerCards(message.message.cards);
            Console.WriteLine(bot?.ToString());
        }

        BotInterface GetBot(string tableId) {
            var bot = bots.Find(b => { return b.GetTableId() == tableId; });
            if (bot == null) {
                XmppManager.Instance.RequestState(tableId);
            }

            return bot;
        }

        public void CloseTable(CloseTable message) {
            bots.Remove(GetBot(message.tableId));
        }//CloseTable
    }//BotManager
}
