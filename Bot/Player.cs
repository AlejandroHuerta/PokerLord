using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot {
    class Player {
        public enum Action { Out = 0, Check = 1, Call = 2, Raise = 3, Bet = 4, Fold = 5, AllIn = 6 }

        public string Name { get; private set; }
        public List<string> Cards = new List<string>();
        public bool PlayingAs { get; private set; }
        public Action LastAction { get; set; }
        public int SeatNumber { get; private set; }
        public int Balance { get; set; }

        public Player(string name, bool playingAs, int seatNumber) {
            Name = name;
            Cards.AddRange(new string[] { "**", "**" });
            LastAction = Action.Out;
            PlayingAs = playingAs;
            SeatNumber = seatNumber;
        }

        public static Action DoubleToAction(double value) {
            return (Action)(int)Math.Round(value * 6.0);
        }

        public static double NormalizeAction(Action action) {
            return (int)action / 6.0;
        }

        public double[] GetStateArray() {
            var state = new double[9];
            state.Populate(0);

            state[8] = NormalizeAction(LastAction);

            if (Cards.Exists(card => { return card == "**"; })) {
                return state;
            }

            var hIndex = 0;
            var dIndex = 2;
            var cIndex = 4;
            var sIndex = 6;
            for (int i = 0; i < 2; i++) {
                if (Cards[i].Contains('h')) {
                    state[hIndex] = GetRankValue(Cards[i].Remove(1));
                    hIndex++;
                }
                else if (Cards[i].Contains('d')) {
                    state[dIndex] = GetRankValue(Cards[i].Remove(1));
                    dIndex++;
                }
                else if (Cards[i].Contains('c')) {
                    state[cIndex] = GetRankValue(Cards[i].Remove(1));
                    cIndex++;
                }
                else {
                    state[sIndex] = GetRankValue(Cards[i].Remove(1));
                    sIndex++;
                }//else
            }//for

            return state;
        }//GetStateArray

        double GetRankValue(string rank) {
            switch (rank) {
            case "A":
                return NormalizeRank(14);
            case "K":
                return NormalizeRank(13);
            case "Q":
                return NormalizeRank(12);
            case "J":
                return NormalizeRank(11);
            case "T":
                return NormalizeRank(10);
            default:
                return NormalizeRank(Double.Parse(rank));
            }//switch
        }//GetRankValue

        double NormalizeRank(double value) {
            return (value - 1) / (14 - 1);
        }
    }
}
