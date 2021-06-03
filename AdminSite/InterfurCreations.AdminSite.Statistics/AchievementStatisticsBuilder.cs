using InterfurCreations.AdventureGames.Database.Statistics;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Graph.Store;
using System;
using System.Linq;

namespace InterfurCreations.AdminSite.BackgroundTasks
{
    public class AchievementStatisticsBuilder
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IDatabaseContextProvider _dbContext;
        private readonly IGameRetrieverService _gameRetrieverService;

        public AchievementStatisticsBuilder(IStatisticsService statisticsService, IDatabaseContextProvider dbContext, IGameRetrieverService gameRetrieverService)
        {
            _statisticsService = statisticsService;
            _gameRetrieverService = gameRetrieverService;
            _dbContext = dbContext;
        }

        public void BuildAchievementStatistic(int batchSize)
        {
            var games = _gameRetrieverService.ListGames();

            var achievements = _statisticsService.ListAchievements();
            var position = _statisticsService.GetStatisticsPosition("Achievements");

            if(position == null)
            {
                position = _dbContext.GetContext().StatisticsPositions.Add(new StatisticsPosition
                {
                    StatisticsName = "Achievements",
                    StatisticsValue = "0"
                }).Entity;
            }

            var gameAchievements = _dbContext.GetContext().PlayerSavedData.Skip(int.Parse(position.StatisticsValue)).Take(batchSize).ToList();

            var dataGroups = gameAchievements.GroupBy(a => a.DataValue);

            foreach(var group in dataGroups)
            {
                var foundGame = games.FirstOrDefault(a => a.Metadata.Achievements.Any(b => b.Name == group.Key));
                if(foundGame != null)
                {
                    var countedItems = group.Count();
                    var existingDbStatisticsEntry = achievements.FirstOrDefault(a => a.AchievementName == group.Key && foundGame.GameName == a.GameName);
                    if(existingDbStatisticsEntry == null)
                    {
                        _dbContext.GetContext().StatisticsGameAchievements.Add(new StatisticsGameAchievement
                        {
                            AchievementName = group.Key,
                            GameName = foundGame.GameName,
                            TotalPlayed = countedItems
                        });
                    } else
                    {
                        existingDbStatisticsEntry.TotalPlayed = existingDbStatisticsEntry.TotalPlayed + countedItems;
                    }
                }
            }

            position.StatisticsValue = (int.Parse(position.StatisticsValue) + gameAchievements.Count).ToString();

            Console.WriteLine("Updated: " + gameAchievements.Count);

            _dbContext.GetContext().SaveChanges();
        }
    }
}
