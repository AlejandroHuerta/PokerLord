//To make this work we're going to need a good model for the state of the game
//We have 52 cards + 1 where the card is unknown.
//Cards go from 2 to 14 (J=11, Q=12, K=13, A=14) and we will use 1 as our unknown
//those numbers must be normalized by (card - 1) / (14 - 1)
//another thing to consider is that there are 4 different suits of cards. These will
//consist of a completely separate input, ie h1, h2, d1, d2, c1, c2, s1, s2
//where each input can be 0-1 after the card is normalized
//The next consideration for our game state is what move each player has done up to us.
//We have the following actions: Check, Call, Raise, Bet, Fold, All-In, these will be numbered
//0 to 5. We must also consider that a player may be already out at the current state.
//We will set the state Out as 6.
//so our normalize function looks like (action - 0) / (6 - 0) or action / 6
//We must also include the table cards which will be as
//tHCard0, tHCard1, tHCard2, tHCard3, tHCard4, tDCard0, tDCard1, tDCard2, ..., tSCard3, tSCard4
//These are 20 inputs in total
//This now sets our complete state as:
//[mHCard0,  mHCard1,  mDCard0,  mDCard1,  mCCard0,  mCCard1,  mSCard0,  mSCard1,  mAction
// p1Action,
// p2Action,
// p3Action,
//...
// p8Action,
//tableCards x 20,
//allowedActions x 3]
//We need 39 input nodes with this setup and 1 output. The one ouput will have a range of action / 5
//actions that we can perform.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser {
    public class Round {
        public double[] State { get; private set; } 

        public Round(List<Player> players, List<string> tableCards) {
            State = BuildTrainingState(players, tableCards);
        }//Round

        public static double[] BuildTrainingState(List<Player> players, List<string> tableCards) {
            var state = new double[40];
            state.Populate(0);

            var playingAs = players.Find(player => { return player.PlayingAs; });
            players.Remove(playingAs);

            //First populate with the our state
            var playingAsState = playingAs.GetStateArray();
            for (int i = 0; i < 9; i++) {
                state[i] = playingAsState[i];
            }

            var otherPlayersArray = GetOtherPlayersArray(players);
            for(int i = 0; i < otherPlayersArray.Length; i++) {
                state[9 + i] = otherPlayersArray[i];
            }

            //Now populate the table Cards
            var tableArray = GetTableArray(tableCards);
            for (int i = 0; i < tableArray.Length; i++) {
                state[17 + i] = tableArray[i];
            }//for

            //Populate allowedActions
            var allowedActions = AllowedActionsAsDouble(players);
            for(int i = 0; i < 3; i++) {
                state[37 + i] = allowedActions[i];
            }//for

            return state;
        }

        public static double[] GetOtherPlayersArray(List<Player> otherPlayers) {
            var state = new double[8];

            otherPlayers.Sort((x, y) => { return Player.NormalizeAction(x.LastAction).CompareTo(Player.NormalizeAction(y.LastAction)); });
            //Now populate everyone else's state
            for (int i = 0; i < otherPlayers.Count; i++) {
                var playerState = Player.NormalizeAction(otherPlayers[i].LastAction);
                state[i] = playerState;
            }//for

            return state;
        }

        public static double[] GetTableArray(List<string> tableCards) {
            var cards = new double[20];
            var hIndex = 0;
            var dIndex = 5;
            var cIndex = 10;
            var sIndex = 15;

            foreach (var c in tableCards) {
                if (c.Contains('h')) {
                    cards[hIndex] = Player.GetRankValue(c.Remove(1));
                    hIndex++;
                } else if (c.Contains('d')) {
                    cards[dIndex] = Player.GetRankValue(c.Remove(1));
                    dIndex++;
                } else if (c.Contains('c')) {
                    cards[cIndex] = Player.GetRankValue(c.Remove(1));
                    cIndex++;
                } else {
                    cards[sIndex] = Player.GetRankValue(c.Remove(1));
                    sIndex++;
                }//else
            }//foreach

            return cards;
        }//GetTableArray

        public string GetStateAsString() {
            return String.Join<double>(",", State);
        }

        public static Player.Action[] AllowedActions(List<Player> otherPlayers) {
            Player.Action[] allowedActions;
            if (otherPlayers.Exists(p => p.LastAction == Player.Action.AllIn)) {
                allowedActions = new Player.Action[3] { Player.Action.Fold, Player.Action.Call, Player.Action.AllIn };
            } else if (otherPlayers.Exists(p => p.LastAction == Player.Action.Bet) ||
                otherPlayers.Exists(p => p.LastAction == Player.Action.Raise) ||
                otherPlayers.Exists(p => p.LastAction == Player.Action.Posted)) {
                allowedActions = new Player.Action[3] { Player.Action.Fold, Player.Action.Call, Player.Action.Raise };
            } else {
                allowedActions = new Player.Action[3] { Player.Action.Fold, Player.Action.Check, Player.Action.Bet };
            }//else

            return allowedActions;
        }//AllowedActions

        public static double[] AllowedActionsAsDouble(List<Player> otherPlayers) {
            var allowedActions = AllowedActions(otherPlayers);
            return new double[3] { Player.NormalizeAction(allowedActions[0]), Player.NormalizeAction(allowedActions[1]), Player.NormalizeAction(allowedActions[2]) };
        }//AllowedActionsAsDouble
    }//Round
}//LogParser
