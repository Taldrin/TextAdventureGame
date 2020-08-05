using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Graph
{
    public static class DrawGameExtensions
    {
        public static DrawState FindStateById(this DrawGame game, string stateId)
        {
            var statesFound = game.Stats.states.Where(a => a.Id == stateId).ToList();
            if(statesFound == null || statesFound.Count == 0)
                throw new Exception("Could not find a state matching stateID: " + stateId + " in game: " + game.GameName + ". It may have been removed, or your save state corrupted.");
            if (statesFound.Count > 1)
                throw new Exception("Found more than 1 state matching stateID: " + stateId + " in game: " + game.GameName + " - this should never happen.");

            return statesFound.First();
        }

        public static StateOption FindOptionById(this DrawGame game, string optionId)
        {
            var optionFound = game.Stats.options.Where(a => a.Id == optionId).ToList();
            if(optionFound == null || optionFound.Count == 0)
                return null;
            if (optionFound.Count > 1)
                return null;

            return optionFound.First();
        }
    }
}
