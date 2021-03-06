﻿using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Graph.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.BackgroundTasks.Tasks
{
    public class GamesByPlayerCountStatisticsBuildTask : IBackgroundTask
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IDatabaseContextProvider _dbContext;
        private readonly IGameRetrieverService _gameRetrieverService;

        public GamesByPlayerCountStatisticsBuildTask(IStatisticsService statisticsService, IDatabaseContextProvider dbContext, IGameRetrieverService gameRetrieverService)
        {
            _statisticsService = statisticsService;
            _gameRetrieverService = gameRetrieverService;
            _dbContext = dbContext;
        }

        public void Run()
        {
            new GamesByPlayerCountStatisticsBuilder(_statisticsService, _dbContext, _gameRetrieverService).Build();
        }
    }
}
