using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.GameTesting
{
    public class GameTestMessageResult
    {
        public string OptionTaken { get; set; }
        public string GameMessage { get; set; }
        public List<string> OptionsPresented { get; set; }
    }
}
