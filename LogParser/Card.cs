using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser {
    class Card {
        public static double[] GetCardArray(List<string> cardsList, int maxCards) {
            var cards = new double[maxCards * 4];
            var hIndex = 0;
            var dIndex = maxCards;
            var cIndex = maxCards * 2;
            var sIndex = maxCards * 3;

            foreach (var c in cardsList) {
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
        }
    }
}
