using InterfurCreations.AdventureGames.Database.Statistics;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Graph.Store;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.BackgroundTasks
{
    public class GamesByPlayerCountStatisticsBuilder
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IDatabaseContextProvider _dbContext;
        private readonly IGameRetrieverService _gameRetrieverService;

        public GamesByPlayerCountStatisticsBuilder(IStatisticsService statisticsService, IDatabaseContextProvider dbContext, IGameRetrieverService gameRetrieverService)
        {
            _statisticsService = statisticsService;
            _gameRetrieverService = gameRetrieverService;
            _dbContext = dbContext;
        }

        public void Build()
        {
            int batchNumber = 0;
            int batchSize = 100;

            var games = _gameRetrieverService.ListGames();

            var gamePlaysDict = new Dictionary<string, long>();

            while(true)
            {
                var players = _dbContext.GetContext().Players.Skip(batchNumber * batchSize).Take(batchSize).Include(a => a.Actions).ToList();
                batchNumber++;

                if (players.Count == 0)
                    break;

                foreach(var player in players) 
                {
                    foreach(var game in games)
                    {
                        var action = player.Actions.FirstOrDefault(a => a.PlayerId == player.PlayerId && a.GameName == game.GameName);
                        if (action != null)
                            AddOrUpdate(gamePlaysDict, game.GameName);
                    }
                }
            }

            var existingStats = _dbContext.GetContext().StatisticsGamesByPlayerCount.ToList();
            foreach (var stat in existingStats)
                _dbContext.GetContext().Remove(stat);

            foreach(var game in gamePlaysDict)
            {
                _dbContext.GetContext().StatisticsGamesByPlayerCount.Add(new StatisticsGamesByPlayerCount
                {
                    GameName = game.Key,
                    TotalPlayed = game.Value
                });
            }
            _dbContext.GetContext().SaveChanges();
        }

        private void AddOrUpdate(Dictionary<string, long> dict, string name)
        {
            if(dict.TryGetValue(name, out var currentAmount))
            {
                dict[name] = currentAmount + 1;
            } else
            {
                dict.Add(name, 1);
            }
        }
    }
}
