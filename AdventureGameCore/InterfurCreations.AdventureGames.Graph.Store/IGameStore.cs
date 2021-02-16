
using System;
using System.Collections.Generic;

namespace InterfurCreations.AdventureGames.Graph.Store
{
    public interface IGameStore
    {
        List<string> CheckForOutOfDateGames(Dictionary<DrawGame, DateTime> timeRetrievedGames);
        byte[] GetGame(string game);
    }
}
