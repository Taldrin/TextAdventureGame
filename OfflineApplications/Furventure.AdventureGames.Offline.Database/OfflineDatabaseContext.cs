using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Database.GameTesting;
using InterfurCreations.AdventureGames.Database.Statistics;
using Microsoft.EntityFrameworkCore;
using System;

namespace Furventure.AdventureGames.Offline.Database
{
    public class OfflineDatabaseContext : DatabaseContext
    {
        private readonly string DbPath;

        public OfflineDatabaseContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}furventure_games.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameSaves>().HasKey(a => new { a.PlayerGameSaveId, a.PlayerId });

            modelBuilder.Ignore<TelegramPlayer>();
            modelBuilder.Ignore<DiscordPlayer>();
            modelBuilder.Ignore<KikPlayer>();
            modelBuilder.Ignore<WebPlayer>();
            modelBuilder.Ignore<AccessToken>();
            modelBuilder.Ignore<StatisticsGameAchievement>();
            modelBuilder.Ignore<StatisticsPosition>();
            modelBuilder.Ignore<StatisticsGamesByPlayerCount>();
            modelBuilder.Ignore<GameTestingEndState>();
            modelBuilder.Ignore<GameTestingError>();
            modelBuilder.Ignore<GameTestingGameSave>();
            modelBuilder.Ignore<GameTestingGameSaveData>();
            modelBuilder.Ignore<GameTestingGrammar>();
            modelBuilder.Ignore<GameTestingMiscData>();
            modelBuilder.Ignore<GameTestingOptionVisited>();
            modelBuilder.Ignore<GameTestingStateVisited>();
            modelBuilder.Ignore<GameTestingWarning>();
        }
    }
}
