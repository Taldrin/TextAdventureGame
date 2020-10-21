using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfurCreations.AdventureGames.DatabaseServices
{
    public class ReportsService : IReportsService
    {
        private readonly IDatabaseContextProvider _contextProvider;

        public ReportsService(IDatabaseContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public Dictionary<string, int> CountActionsByGame(DateTime after, DateTime before)
        {
            return _contextProvider.GetContext().PlayerActions.Where(a => a.Time > after && a.Time < before).ToList().GroupBy(a => a.GameName).ToDictionary(a => a.Key, a => a.Count());
        }

        public int CountPlayers(DateTime after, DateTime before)
        {
            return _contextProvider.GetContext().PlayerActions.Where(a => a.Time > after && a.Time < before).ToList().GroupBy(a => a.PlayerId).Count();
        }

        public int CountTotalPlayers()
        {
            return _contextProvider.GetContext().Players.Count();
        }

        public int CountTotalActions()
        {
            return _contextProvider.GetContext().PlayerActions.Count();
        }

        public int CountTotalGameSaves()
        {
            return _contextProvider.GetContext().PlayerGameSave.Count();
        }
    }
}
