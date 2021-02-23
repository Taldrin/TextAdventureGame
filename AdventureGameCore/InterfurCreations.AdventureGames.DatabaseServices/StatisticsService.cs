using InterfurCreations.AdventureGames.Database.Statistics;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfurCreations.AdventureGames.DatabaseServices
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IDatabaseContextProvider _databaseContext;

        public StatisticsService(IDatabaseContextProvider databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<StatisticsGameAchievement> ListAchievements()
        {
            return _databaseContext.GetContext().StatisticsGameAchievements.ToList();
        }

        public StatisticsPosition GetStatisticsPosition(string statisticsName)
        {
            return _databaseContext.GetContext().StatisticsPositions.FirstOrDefault(a => a.StatisticsName == statisticsName);
        }

        public int CountTotalPlayers()
        {
            return _databaseContext.GetContext().Players.Count();
        }
    }
}
