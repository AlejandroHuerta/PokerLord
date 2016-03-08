using System;
using System.Collections.Generic;
using Bot.Messages;
using LogParser;
using System.Linq;

namespace Bot {
    class Bot : BotManager.BotInterface {
        static Random random = new Random();

        public string TableId { get; private set; }

        List<string> tableCards;

        string playingAs;

        List<Player> players = new List<Player>();
        public List<Player> Players {
            get {
                return players;
            }
        }

        int minBet = 0;

        int pot = 0;

        public Bot(string tableId, string username) {
            TableId = tableId;
            playingAs = username;
            tableCards = new List<string>();
        }

        public string GetTableId() {
            return TableId;
        }

        public void ResetActions() {
            foreach (var player in players) {
                player.LastAction = Player.Action.Out;
            }
        }

        public void ClearContributions() {
            players.ForEach(p => p.Contribution = 0);
        }//ClearContributions

        public void SetPlayers(List<StatePlayer> players) {
            //Addition
            foreach (var player in players) {
                if (!this.players.Exists(p => p.Name == player.username)) {
                    var playerObject = new Player(player.username, player.username == playingAs, player.seatNumber);
                    playerObject.Balance = player.balance;
                    this.players.Add(playerObject);
                }//if
            }//foreach

            //Subtraction
            foreach (var player in this.players.Reverse<Player>()) {
                if (!players.Exists(p => p.username == player.Name)) {
                    this.players.Remove(player);
                }//if
            }//foreach
        }//SetPlayers

        public void ActivePlayer(int seat, float timeout) {
            var activePlayer = players.Find(player => { return player.SeatNumber == seat; });
            if (activePlayer != null) {
                Console.WriteLine(activePlayer.Name + " is active.");
                if (activePlayer.PlayingAs) {
                    Act(timeout);
                }//if
            }//if
        }//ActivePlayer

        public void SetPlayerAction(int seat, Player.Action action) {
            var player = players.Find(p => { return p.SeatNumber == seat; });
            player.LastAction = action;
        }

        public void SetContribution(int seat, int amount) {
            players.Find(p => p.SeatNumber == seat).Contribution = amount;
        }

        public void SetBalance(int seat, int balance) {
            var player = players.Find(p => { return p.SeatNumber == seat; });
            player.Balance = balance;
        }

        public void SetMinimumBet(int minBet) {
            this.minBet = minBet;
        }

        public void TableCards(List<string> cards) {
            tableCards = cards;
        }

        public void ClearTableCards() {
            tableCards.Clear();
        }

        public void PlayerCards(List<List<string>> cards) {
            for (int i = 0; i < cards.Count; i++) {
                if (cards[i] != null) {
                    var player = players.SingleOrDefault(p => p.SeatNumber == i);
                    if (player != null) {
                        player.Cards = cards[i];
                    }//if
                }//if
            }//for
        }//PlayerCards

        public void AddPot(int amount) {
            pot += amount;
        }//SetPot

        public void ResetPot() {
            pot = 0;
        }//ResetPot

        void Act(float timeout) {
            var state = Round.BuildState(new List<Player>(players), tableCards, pot);

            var computed = HiveMind.Instance.Compute(state);
            var action = Player.DoubleToIdeal(computed);
            Console.WriteLine("Computed action: {0}", action);

            var tempPlayers = new List<Player>(players);
            var playingAs = tempPlayers.Find(p => p.PlayingAs);
            tempPlayers.Remove(playingAs);

            if (!Round.GetAllowedActions(playingAs, tempPlayers).Contains(action)) {
                Player.Action translation;
                switch(action) {
                case Player.Action.Bet:
                    translation = Player.Action.Raise;
                    break;
                case Player.Action.Raise:
                    translation = Player.Action.Bet;
                    break;
                case Player.Action.Check:
                    translation = Player.Action.Call;
                    break;
                case Player.Action.Call:
                    translation = Player.Action.Check;
                    break;
                default:
                    translation = Player.Action.Fold;
                    break;
                }//switch
                Console.WriteLine("{0,-6} is not an allowed action! Action will be translated to {1}", action, translation);
                action = translation;
            }//if

            int amount = 0;
            switch (action) {
            case Player.Action.AllIn:
                amount = players.Find(player => player.PlayingAs).Balance;
                break;
            case Player.Action.Bet:
                amount = minBet;
                break;
            case Player.Action.Raise:
                amount = minBet * 2;
                break;
            case Player.Action.Out:
                action = Player.Action.Fold;
                Console.WriteLine("Out should not be possible! HiveMind returned {0}", computed);
                break;
            }//switch

            var sleepTime = (int)(random.NextDouble() * timeout * 1000);
            Console.WriteLine("Waiting for {0}ms before acting...", sleepTime);
            System.Threading.Thread.Sleep(sleepTime);

            XmppManager.Instance.SendAction(action, TableId, amount);
        }//Act

        public override string ToString() {
            return String.Format("Table Cards: {0,-20}", '[' + string.Join("][", tableCards) + ']') + "\n" + string.Join("\n", players.Select(p => p.ToString()));
        }
    }//Bot
}//Bot
