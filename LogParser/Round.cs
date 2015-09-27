using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser {
    public class Round {
        public double[] State { get; private set; }

        public Round(List<Player> players, List<string> tableCards, Player.Action action) {
            State = BuildTrainingState(players, tableCards, action);
        }//Round

        //[mLastAction, p1LastAction, ..., p8LastAction,
        // mCards,
        // tableCards]
        //[9, 8, 20]
        public static double[] BuildState(List<Player> players, List<string> tableCards) {
            var state = new double[1];

            var playingAs = players.Find(player => { return player.PlayingAs; });
            players.Remove(playingAs);

            //First populate with the our state
            state[0] = Player.NormalizeAction(playingAs.LastAction);

            //Populate with other players states
            var otherPlayersArray = GetOtherPlayersArray(players);
            state = state.Concat(otherPlayersArray).ToArray();

            //Populate with our cards
            var mCardsArray = Card.GetCardArray(playingAs.Cards, 2);
            state = state.Concat(mCardsArray).ToArray();

            //Now populate the table Cards
            var tableArray = Card.GetCardArray(tableCards, 5);
            state = state.Concat(tableArray).ToArray();

            return state;
        }//BuildState

        //[BuildState,
        // idealAction]
        //[, 1] 
        public static double[] BuildTrainingState(List<Player> players, List<string> tableCards, Player.Action IdealAction) {
            var state = BuildState(players, tableCards);

            //Populate with ideal action
            state = state.Concat(new double[1] { Player.NormalizeIdeal(IdealAction) }).ToArray();

            return state;
        }//BuildTrainingState

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

        public string GetStateAsString() {
            return String.Join<double>(",", State);
        }
    }//Round
}//LogParser
