using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.Graph.Store
{
    public interface IGameRetrieverService
    {
        List<DrawGame> ListGames(TimeSpan timeBetweenCheck, bool includeFilteredGames = false);
        List<DrawGame> ListGames(bool includeFilteredGames = false);
    }
}
