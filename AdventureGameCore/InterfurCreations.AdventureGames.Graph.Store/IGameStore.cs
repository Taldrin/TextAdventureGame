
using System.Collections.Generic;

namespace InterfurCreations.AdventureGames.Graph.Store
{
    public interface IGameStore
    {
        void Initialise();
        List<DrawGame> ListGames();
        List<DrawGame> ListGames(System.TimeSpan timeBetweenCheck);
    }
}
