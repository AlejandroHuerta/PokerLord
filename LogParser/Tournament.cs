using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogParser {
    class Tournament {
        enum HandSearchState { Looking, Adding };

        public string Id { get; private set; }
        public List<string> Tables { get; private set; }
        public string OverallWinner { get; private set; }

        string logPath;
        public List<Hand> hands;

        public Tournament(string logPath, string[] files) {
            this.logPath = logPath;
            Id = (new Regex(@"[0-9a-z]{8}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{12}")).Match(logPath).Value;

            FindOverallWinner();
            GetTables(files);
            GenerateHands();
        }//Tournament

        void GetTables(string[] files) {
            var regex = new Regex(Id + "_[0-9].log");
            Tables = files.Where<string>(file => { return regex.IsMatch(file); }).ToList<string>();
        }//GetTables

        void FindOverallWinner() {
            var regex = new Regex(@"Player [0-9a-z]+ was eliminated an finished in rank 0$");
            var lines = File.ReadLines(logPath);
            foreach (var line in lines) {
                var match = regex.Match(line);
                if (match.Success) {
                    OverallWinner = match.Value.Replace("Player ", "").Replace(" was eliminated an finished in rank 0", "");
                }//if
            }//foreach
        }//FindOverallWinner

        void GenerateHands() {
            hands = new List<Hand>();

            var lines = new List<string>();
            HandSearchState state = HandSearchState.Looking;

            var beginOfHandRegex = new Regex(@"The players in this hand:");
            var endOfHandRegex = new Regex(@"Time until stakes level change");
            foreach (var table in Tables) {
                var fileLines = File.ReadLines(table);
                foreach (var line in fileLines) {
                    switch (state) {
                    case HandSearchState.Looking:
                        if (beginOfHandRegex.IsMatch(line)) {
                            state = HandSearchState.Adding;
                        }//if
                        break;
                    case HandSearchState.Adding:
                        if (endOfHandRegex.IsMatch(line)) {
                            hands.Add(new Hand(lines));
                            lines = new List<string>();
                            state = HandSearchState.Looking;
                        }
                        else {
                            lines.Add(line);
                        }//else
                        break;
                    }//switch
                }//foreach line
            }//foreach table            
        }//GenerateHands
    }//Tournament
}//LogParser
