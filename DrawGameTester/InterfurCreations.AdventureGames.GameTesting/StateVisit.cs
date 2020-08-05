using InterfurCreations.AdventureGames.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.GameTesting
{
    public class StateVisit
    {
        public string StateId { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public List<PlayerGameSaveData> SaveData { get; set; }
    }
}
