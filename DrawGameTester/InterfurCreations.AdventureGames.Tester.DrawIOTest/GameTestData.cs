using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Tester.DrawIOTest
{
    public class GameTestData
    {
        public Dictionary<string, int> StateVisits;
        public Dictionary<string, int> OptionVisits;

        public GameTestData()
        {
            StateVisits = new Dictionary<string, int>();
            OptionVisits = new Dictionary<string, int>();
        }

        public void StateVisited(string id)
        {
            if (StateVisits.TryGetValue(id, out int value))
                StateVisits[id] = value++;
            else
                StateVisits.Add(id, 1);
        }

        public void OptionChosen(string id)
        {
            if (OptionVisits.TryGetValue(id, out int value))
                OptionVisits[id] = value++;
            else
                OptionVisits.Add(id, 1);
        }

        public int GetTimesChosen(string id)
        {
            if (OptionVisits.TryGetValue(id, out int value))
                return OptionVisits[id];
            else
                return 0;
        }

        public List<string> GetAllStatesVisited()
        {
            return StateVisits.Keys.ToList();
        }
    }
}
