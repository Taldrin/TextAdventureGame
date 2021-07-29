using InterfurCreations.AdventureGames.Database.Statistics;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furventure.AdventureGames.Offline.Core
{
    public class EmptyStatisticsService : IStatisticsService
    {
        public int CountTotalPlayers()
        {
            return 1;
        }

        public StatisticsGamesByPlayerCount GetPlayerCountForGame(string gameName)
        {
            return new StatisticsGamesByPlayerCount
            {
                GameName = gameName,
                TotalPlayed = 1
            };
        }

        public StatisticsPosition GetStatisticsPosition(string statisticsName)
        {
            return new StatisticsPosition { StatisticsName = statisticsName, StatisticsValue = "1" };
        }

        public List<StatisticsGameAchievement> ListAchievements()
        {
            return new List<StatisticsGameAchievement> { };
        }
    }
}
