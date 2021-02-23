using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Graph.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.Statistics.Tasks
{
    public class AchievementStatisticsBuildTask
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IDatabaseContextProvider _dbContext;
        private readonly IGameRetrieverService _gameRetrieverService;

        public AchievementStatisticsBuildTask(IStatisticsService statisticsService, IDatabaseContextProvider dbContext, IGameRetrieverService gameRetrieverService)
        {
            _statisticsService = statisticsService;
            _gameRetrieverService = gameRetrieverService;
            _dbContext = dbContext;
        }

        public void Run()
        {
            new AchievementStatisticsBuilder(_statisticsService, _dbContext, _gameRetrieverService).BuildAchievementStatistic(500);
        }
    }
}
