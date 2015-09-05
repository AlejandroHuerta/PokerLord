using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogParser {
    class Program {
        static void Main(string[] args) {
            string[] filePaths = Directory.GetFiles(@"G:/game_logs_aug15/game_logs_aug15/logs/", "*.log", SearchOption.AllDirectories);
            var tournaments = new List<Tournament>();

            var regex = new Regex(@"[0-9a-z]{8}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{12}.log$");
            foreach(var filePath in filePaths) {
                var match = regex.Match(filePath);
                if (match.Success) {
                    tournaments.Add(new Tournament(filePath, filePaths));
                }//if
                break;
            }//foreach

            using (var file = new StreamWriter("./states.txt")) {
                foreach (var tournament in tournaments) {
                    foreach (var hand in tournament.hands) {
                        foreach (var round in hand.rounds) {
                            file.WriteLine(round.GetStateAsString());
                        }
                    }
                }
            }

            Console.WriteLine("states.txt written!");
            Console.ReadKey();
        }
    }
}
