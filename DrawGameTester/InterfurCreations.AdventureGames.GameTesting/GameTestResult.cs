using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.GameTesting
{
    public class GameTestResult
    {
        public HashSet<string> ErrorMessages { get; set; }
        public HashSet<string> MessagesRecieved { get; set; }
        public HashSet<string> WarningMessages { get; set; }
        public int totalActionsDone { get; set; }
    }
}