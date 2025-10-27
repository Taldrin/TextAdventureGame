using InterfurCreations.AdventureGames.Database.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services
{
    public class AIStateService
    {
        public Dictionary<string, AIState> SavedAIStates = new Dictionary<string, AIState>();

        public void SaveNewState(string playerId, AIState state)
        {
            //if (SavedAIStates.TryGetValue(playerId, out var newState))
            //{
            //    SavedAIStates[playerId] = newState;
            //}
            SavedAIStates[playerId] = state;
        }

        public AIState GetAIState(string playerId)
        {
            if (SavedAIStates.TryGetValue(playerId, out var state))
            {
                return state;
            }

            return null;
        }
        public AIState DeleteAIState(string playerId)
        {
            if (SavedAIStates.Remove(playerId, out var state))
            {
                return state;
            }

            return null;
        }
    }
}
