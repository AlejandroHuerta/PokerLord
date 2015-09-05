using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogParser {
    class Hand {
        static Regex winRegex = new Regex(@"[a-z0-9]+ wins");
        static Regex playerRegex = new Regex(@"[a-z0-9]+ : \[");
        static Regex dealerPositionRegex = new Regex(@"Dealer position is: \d$");

        static Regex postedRegex = new Regex(@"[a-z0-9]+ posted \$[0-9]+.$");

        static Regex isActionRegex = new Regex(@"[a-z0-9]+ (folded|went all-in with \$[0-9]+|called \$[0-9]+|checked|bet \$[0-9]+|raised to \$[0-9]+).$");
        static Regex foldedRegex = new Regex(@"[a-z0-9]+ folded.$");
        static Regex allinRegex = new Regex(@"[a-z0-9]+ went all-in with \$[0-9]+.$");
        static Regex calledRegex = new Regex(@"[a-z0-9]+ called \$[0-9]+.");
        static Regex checkedRegex = new Regex(@"[a-z0-9]+ checked.$");
        static Regex betRegex = new Regex(@"[a-z0-9]+ bet \$[0-9]+.$");
        static Regex raisedRegex = new Regex(@"[a-z0-9]+ raised to \$[0-9]+.$");

        static Regex nameRegex = new Regex(@": [a-z0-9]+");

        static Regex dealingRegex = new Regex(@"Dealing ");

        public List<Round> rounds { get; private set; }

        List<string> tableCards = new List<string>();
        List<Player> players = new List<Player>();

        string playingAs;

        delegate void LineParser(string line);
        LineParser currentParser;

        int postedCount = 2;

        public Hand(List<string> lines) {
            playingAs = "n/a";
            rounds = new List<Round>();

            WhoWon(lines);
            Parse(lines);
        }//Hand

        void WhoWon(List<string> lines) {
            foreach (var line in lines.Reverse<string>()) {
                var match = winRegex.Match(line);
                if (match.Success) {
                    playingAs = match.Value.Replace(" wins", "");
                    break;
                }//if
            }//foreach
        }//WhoWon

        void Parse(List<string> lines) {
            currentParser = GettingPlayersParser;
            foreach (var line in lines) {
                currentParser(line);
            }//foreach
        }//Parse

        void GettingPlayersParser(string line) {
            var match = playerRegex.Match(line);
            if (match.Success) {
                var name = match.Value.Replace(" : [", "");
                Player player;
                if (name == playingAs) {
                    player = new Player(name, true);
                    player.Cards = ExtractCards(line);
                }
                else {
                    player = new Player(name, false);
                }
                players.Add(player);
            }
            else if (dealerPositionRegex.IsMatch(line)) {
                currentParser = IgnorePostedParser;
            }//else if
        }//GettingPlayersParser

        void IgnorePostedParser(string line) {
            if (postedRegex.IsMatch(line)) {
                if (postedCount == 1) {
                    currentParser = BuildStateParser;
                }
                else {
                    postedCount--;
                }//else
            }//if
        }//IgnorePostedParser

        void BuildStateParser(string line) {
            string name;
            Player.Action action;

            if (isActionRegex.IsMatch(line)) {
                name = nameRegex.Match(line).Value.Replace(": ", "");
                if (foldedRegex.IsMatch(line)) {
                    action = Player.Action.Fold;
                } else if (allinRegex.IsMatch(line)) {
                    action = Player.Action.AllIn;
                } else if (calledRegex.IsMatch(line)) {
                    action = Player.Action.Call;
                } else if (checkedRegex.IsMatch(line)) {
                    action = Player.Action.Check;
                } else if (betRegex.IsMatch(line)) {
                    action = Player.Action.Bet;
                } else if (raisedRegex.IsMatch(line)) {
                    action = Player.Action.Raise;
                } else {
                    action = Player.Action.Out;
                    Console.WriteLine("Unknown action performed!");
                    Console.WriteLine(line);
                }//else

                players.Find(player => { return player.Name == name; }).LastAction = action;

                if (name == playingAs) {
                    rounds.Add(new Round(new List<Player>(players), tableCards));
                }//if
            } else if (dealingRegex.IsMatch(line)) {
                tableCards.AddRange(ExtractCards(line));

                players.ForEach(player => { player.LastAction = Player.Action.Out; });
            }//else if
        }//BuildStateParser

        List<string> ExtractCards(string line) {
            var cards = new List<string>();
            var matches = Regex.Matches(line, @"[2-9TJQKA][shcd]");

            foreach (Capture capture in matches) {
                cards.Add(capture.Value);
            }

            return cards;
        }
    }//Hand
}//LogParser
