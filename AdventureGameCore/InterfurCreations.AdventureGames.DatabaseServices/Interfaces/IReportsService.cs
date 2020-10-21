using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.DatabaseServices.Interfaces
{
    public interface IReportsService
    {
        Dictionary<string, int> CountActionsByGame(DateTime after, DateTime before);
        int CountPlayers(DateTime after, DateTime before);
        int CountTotalActions();
        int CountTotalGameSaves();
        int CountTotalPlayers();
    }
}
