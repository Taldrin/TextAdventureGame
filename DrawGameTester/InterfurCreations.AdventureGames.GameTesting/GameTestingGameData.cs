using InterfurCreations.AdventureGames.Database.GameTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.GameTesting
{
    public class GameTestingGameData
    {
        public List<GameTestingEndState> EndStates { get; set; }
        public List<GameTestingError> Errors { get; set; }
        public List<GameTestingGrammar> GrammarWarnings { get; set; }
        public List<GameTestingMiscData> MiscData { get; set; }
        public List<GameTestingOptionVisited> OptionsVisited { get; set; }
        public List<GameTestingStateVisited> StatesVisited { get; set; }
        public List<GameTestingWarning> Warnings { get; set; }
    }
}
