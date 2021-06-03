using InterfurCreations.AdventureGames.Graph;
using System;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.GameTesting
{

    public interface IGameTestExecutor
    {
        Task RunTestAsync(DrawGame drawGame, DateTime runUntil, int actionsPerRunOption, GameTestDataStore dataStore, string startingStateId = null);
    }
}
