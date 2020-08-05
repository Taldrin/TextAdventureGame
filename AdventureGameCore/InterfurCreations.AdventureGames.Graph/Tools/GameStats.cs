using InterfurCreations.AdventureGames.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.BotMain.Tools
{
    public class GameStats
    {
        public int optionsCount;
        public HashSet<DrawState> states;
        public HashSet<StateOption> options;
        public int wordCount;
    }
}
