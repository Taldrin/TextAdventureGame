using InterfurCreations.AdventureGames.Database.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.DatabaseServices.Interfaces
{
    public interface IStatisticsService
    {
        int CountTotalPlayers();
        StatisticsPosition GetStatisticsPosition(string statisticsName);
        List<StatisticsGameAchievement> ListAchievements();
    }
}
