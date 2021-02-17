using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.Graph.Store
{
    public interface IGameRetrieverService
    {
        List<DrawGame> ListGames(TimeSpan timeBetweenCheck);
        List<DrawGame> ListGames();
    }
}
