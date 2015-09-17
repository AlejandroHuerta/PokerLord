using System;
using System.Collections.Generic;
using Bot.Messages;
using LogParser;

namespace Bot {
    class Bot : BotManager.BotInterface {
        public string TableId { get; private set; }

        List<string> tableCards;

        string playingAs;

        List<Player> players = new List<Player>();

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
            Console.WriteLine(activePlayer.Name + " is active.");
            if (activePlayer.PlayingAs) {
                Act();
            }
        }

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

        void Act() {
            var state = new double[39];
            state.Populate(0);

            var tempPlayers = new List<Player>(players);
            var playingAs = tempPlayers.Find(player => { return player.PlayingAs; });
            tempPlayers.Remove(playingAs);

            //First populate with the our state
            var playingAsState = playingAs.GetStateArray();
            for (int i = 0; i < 8; i++) {
                state[i] = playingAsState[i];
            }//for

            var otherPlayersArray = Round.GetOtherPlayersArray(tempPlayers);
            for (int i = 0; i < otherPlayersArray.Length; i++) {
                state[8 + i] = otherPlayersArray[i];
            }//for

            var tableArray = Round.GetTableArray(tableCards);
            for (int i = 0; i < tableArray.Length; i++) {
                state[16 + i] = tableArray[i];
            }//for
            
            var allowedActions = Round.AllowedActionsAsDouble(tempPlayers);
            for (int i = 0; i < 3; i++) {
                state[36 + i] = allowedActions[i];
            }//for

            var computed = HiveMind.Instance.Compute(state);
            var action = Player.DoubleToIdeal(computed);
            Console.WriteLine("Computed action: {0}", action);

            int amount = 0;
            switch (action) {
            case Player.Action.AllIn:
                amount = playingAs.Balance;
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
            }

            XmppManager.Instance.sendAction(action, TableId, amount);
        }
    }
}
