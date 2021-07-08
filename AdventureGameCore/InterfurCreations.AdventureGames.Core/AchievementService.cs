using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfurCreations.AdventureGames.Core
{
    public static class AchievementService
    {
        public static List<(bool hasAchieved, DrawAchievement achievement)> HasPlayerDoneAchievements(DrawGame game, Player player)
        {
            return game.Metadata.Achievements.Select(a => (player.PermanentData.Any(b => b.DataName == game.GameName && b.DataType == PlayerSaveDataType.ACHIEVEMENT.ToString() && b.DataValue.ToLower() == a.Name.ToLower()), a)).ToList();
        }

        public static int CountAchievementsCompletedForGames(List<DrawGame> games, Player player)
        {
            return games.Sum(a => HasPlayerDoneAchievements(a, player).Count(b => b.hasAchieved));
        }

        public static int CountTotalAchievements(List<DrawGame> games)
        {
            return games.Sum(a => a.Metadata.Achievements.Count());
        }

        public static string GetPercentageAchieved(IStatisticsService service, DrawGame game, string achievement)
        {
            var playerCount = service.GetPlayerCountForGame(game.GameName);
            var achievementCount = service.ListAchievements().FirstOrDefault(a => a.GameName == game.GameName && a.AchievementName == achievement)?.TotalPlayed;
            if(achievementCount.HasValue && playerCount != null)
            {
                return " - " + string.Format("{0:0.#}", ((double)achievementCount.Value / (double)playerCount.TotalPlayed) * 100) + "%";
            }
            return string.Empty;
        }
    }
}
