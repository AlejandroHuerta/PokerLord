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
//This now sets our complete state as:
//[mHCard0,  mHCard1,  mDCard0,  mDCard1,  mCCard0,  mCCard1,  mSCard0,  mSCard1,  mAction
// p1HCard0, p1HCard1, p1DCard0, p1DCard1, p1CCard0, p1CCard1, p1SCard0, p1SCard1, p1Action,
// p2HCard0, p2HCard1, p2DCard0, p2DCard1, p2CCard0, p2CCard1, p2SCard0, p2SCard1, p2Action,
// p3HCard0, p3HCard1, p3DCard0, p3DCard1, p3CCard0, p3CCard1, p3SCard0, p3SCard1, p3Action,
//...
// p8HCard0, p8HCard1, p8DCard0, p8DCard1, p8CCard0, p8CCard1, p8SCard0, p8SCard1, p8Action]
//We need 81 input nodes with this setup and 1 output. The one ouput will have a range of action / 5
//actions that we can perform.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser {
    public class Player {
        public enum Action { Check = 0, Call = 1, Raise = 2, Bet = 3, Fold = 4, AllIn = 5, Out = 6, SmallBlind = 7, BigBlind = 8 }

        public string Name { get; private set; }
        public List<string> Cards = new List<string>();
        public bool PlayingAs { get; private set; }
        public Action LastAction { get; set; }
        public int Balance { get; set; }
        public int SeatNumber { get; set; }
        public int Contribution { get; set; }

        public Player(string name, bool playingAs, int seatNumber = 0) {
            Name = name;
            Cards.AddRange(new string[] { "**", "**" });
            LastAction = Action.Out;
            PlayingAs = playingAs;
            SeatNumber = seatNumber;
            Contribution = 0;
        }

        public static Action DoubleToIdeal(double value) {
            return (Action)(int)Math.Round(value * 5.0);
        }

        public static double NormalizeAction(Action action) {
            return (int)action / 8.0;
        }

        public static double NormalizeIdeal(Action ideal) {
            return (int)ideal / 5.0;
        }

        public static double GetRankValue(string rank) {
            switch(rank) {
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

        static double NormalizeRank(double value) {
            return (value - 1) / (14 - 1);
        }

        public override string ToString() {
            return string.Format("Name: {0,-12} Seat Number: {1} Cards: [{2}][{3}] Last Action: {4,-10} Balance: {5,6} Contribution: {6}", Name, SeatNumber, Cards[0], Cards.Count > 1 ? Cards[1] : "--", LastAction, Balance, Contribution);
        }
    }
}
