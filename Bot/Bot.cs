using System;
using System.Collections.Generic;
using Bot.Messages;
using LogParser;
using System.Linq;

namespace Bot {
    class Bot : BotManager.BotInterface {
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

        public void SetPlayers(List<StatePlayer> players) {
            this.players.Clear();

            foreach (var player in players) {
                var playerObject = new Player(player.username, player.username == playingAs, player.seatNumber);
                playerObject.Balance = player.balance;
                this.players.Add(playerObject);
            }
        }

        public void ActivePlayer(int seat) {
            var activePlayer = players.Find(player => { return player.SeatNumber == seat; });
            if (activePlayer != null) {
                Console.WriteLine(activePlayer.Name + " is active.");
                if (activePlayer.PlayingAs) {
                    Act();
                }//if
            }//if
        }//ActivePlayer

        public void SetPlayerAction(int seat, Player.Action action) {
            var player = players.Find(p => { return p.SeatNumber == seat; });
            player.LastAction = action;
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
            for(int i = 0; i < cards.Count; i++) {
                if (cards[i] != null) {
                    players.Find(p => p.SeatNumber == i).Cards = cards[i];
                }
            }//for
        }//PlayerCards

        void Act() {
            var tempPlayers = new List<Player>(players);

            var state = Round.BuildState(tempPlayers, tableCards);

            var computed = HiveMind.Instance.Compute(state);
            var action = Player.DoubleToIdeal(computed);
            Console.WriteLine("Computed action: {0}", action);
            
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

            XmppManager.Instance.sendAction(action, TableId, amount);
        }//Act

        public override string ToString() {
            return String.Format("Table Cards: {0,-20}", '[' + string.Join("][", tableCards) + ']') + "\n" + string.Join("\n", players.Select(p => p.ToString()));
        }
    }//Bot
}//Bot
