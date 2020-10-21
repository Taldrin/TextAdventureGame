using InterfurCreations.AdventureGames.BotMain.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Graph
{
    public class DrawGame
    {
        public string GameName;
        public DrawState StartState;
        public List<DrawGameFunction> GameFunctions;
        public GameStats Stats;
        public DrawMetadata Metadata;
    }
}
