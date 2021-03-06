﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser {
    public class Round {
        const double CHIP_STACK_MAX = 10000000;

        public double[] State { get; private set; }

        public Round(List<Player> players, List<string> tableCards, Player.Action action, int pot) {
            State = BuildTrainingState(players, tableCards, action, pot);
        }//Round

        //[mLastAction, p1LastAction, ..., p8LastAction,
        // mCards,
        // tableCards,
        // uncalled,
        // allowedActions,
        // balances,
        // pot]
        //[9, 8, 20, 1, 4, 9, 1]
        public static double[] BuildState(List<Player> players, List<string> tableCards, int pot) {
            var state = new List<double>();

            var playingAs = players.Find(player => { return player.PlayingAs; });
            players.Remove(playingAs);

            //First populate with the our state
            state.Add(Player.NormalizeAction(playingAs.LastAction));

            //Populate with other players states
            var otherPlayersArray = GetOtherPlayersArray(players);
            state.AddRange(otherPlayersArray);

            //Populate with our cards
            var mCardsArray = Card.GetCardArray(playingAs.Cards, 2);
            state.AddRange(mCardsArray);

            //Now populate the table Cards
            var tableArray = Card.GetCardArray(tableCards, 5);
            state.AddRange(tableArray);

            //Add whether there is an uncalled amount (true or false)
            if (GetUncalledAmount(playingAs, players) > 0) {
                state.Add(1.0);
            } else {
                state.Add(0.0);
            }//else

            //Add allowed actions
            state.AddRange(GetAllowedActionsNormalized(playingAs, players));

            //Add balances
            state.Add(NormalizeChipstack(playingAs.Balance));
            state.AddRange(GetOtherPlayerBalances(players));

            //Add the pot
            state.Add(NormalizeChipstack(pot));

            return state.ToArray();
        }//BuildState

        //[BuildState,
        // idealAction]
        //[, 1] 
        public static double[] BuildTrainingState(List<Player> players, List<string> tableCards, Player.Action IdealAction, int pot) {
            var state = BuildState(players, tableCards, pot);

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
        }//GetOtherPlayersArray

        public static double[] GetOtherPlayerBalances(List<Player> otherPlayers) {
            var state = new double[8];

            otherPlayers.Sort((x, y) => { return Player.NormalizeAction(x.LastAction).CompareTo(Player.NormalizeAction(y.LastAction)); });
            //Now populate everyone else's state
            for (int i = 0; i < otherPlayers.Count; i++) {
                var playerState = NormalizeChipstack(otherPlayers[i].Balance);
                state[i] = playerState;
            }//for

            return state;
        }//GetOtherPlayerBalances

        public static int GetUncalledAmount(Player playingAs, List<Player> otherPlayers) {
            return otherPlayers.Any() ? otherPlayers.Max(p => p.Contribution) - playingAs.Contribution : 0;
        }//GetUncalledAmount

        public static Player.Action[] GetAllowedActions(Player playingAs, List<Player> otherPlayers) {
            if (GetUncalledAmount(playingAs, otherPlayers) > 0) {
                return new Player.Action[4] { Player.Action.Fold, Player.Action.Call, Player.Action.Raise, Player.Action.AllIn };
            } else {
                return new Player.Action[4] { Player.Action.Fold, Player.Action.Check, Player.Action.Bet, Player.Action.AllIn };
            }//else
        }//GetAllowedActionsArray

        public static double[] GetAllowedActionsNormalized(Player playingAs, List<Player> otherPlayers) {
            return GetAllowedActions(playingAs, otherPlayers).Select(a => Player.NormalizeIdeal(a)).ToArray();
        }//GetAllowedActionsNormalized

        public string GetStateAsString() {
            return String.Join<double>(",", State);
        }

        public static double NormalizeChipstack(int chipAmount) {
            return chipAmount / CHIP_STACK_MAX;
        }//NormalizeChipstack
    }//Round
}//LogParser
