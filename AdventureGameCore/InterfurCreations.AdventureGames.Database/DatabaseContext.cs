using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Database.GameTesting;
using InterfurCreations.AdventureGames.Database.Statistics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InterfurCreations.AdventureGames.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly IConfigurationService _configService;

        #region Core Data
        public DbSet<PlayerAction> PlayerActions { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<TelegramPlayer> TelegramPlayers { get; set; }
        public DbSet<DiscordPlayer> DiscordPlayers { get; set; }
        public DbSet<KikPlayer> KikPlayers { get; set; }
        public DbSet<WebPlayer> WebPlayers { get; set; }
        public DbSet<PlayerGameSave> PlayerGameSave { get; set; }
        public DbSet<PlayerGameSaveData> PlayerGameSaveData { get; set; }
        public DbSet<GameSaves> GameSaves { get; set; }
        public DbSet<AccessToken> AccessToken { get; set; }
        public DbSet<PlayerSavedData> PlayerSavedData { get; set; }
        #endregion

        #region Statistics
        public DbSet<StatisticsGameAchievement> StatisticsGameAchievements { get; set; }
        public DbSet<StatisticsPosition> StatisticsPositions { get; set; }
        public DbSet<StatisticsGamesByPlayerCount> StatisticsGamesByPlayerCount { get; set; }
        #endregion

        #region Game Testing
        public DbSet<GameTestingEndState> GameTestingEndStates { get; set; }
        public DbSet<GameTestingError> GameTestingError { get; set; }
        public DbSet<GameTestingGameSave> GameTestingGameSave { get; set; }
        public DbSet<GameTestingGameSaveData> GameTestingGameSaveData { get; set; }
        public DbSet<GameTestingGrammar> GameTestingGrammar { get; set; }
        public DbSet<GameTestingMiscData> GameTestingMiscData { get; set; }
        public DbSet<GameTestingOptionVisited> GameTestingOptionVisited { get; set; }
        public DbSet<GameTestingStateVisited> GameTestingStateVisited { get; set; }
        public DbSet<GameTestingWarning> GameTestingWarning { get; set; }
        #endregion

        public DatabaseContext(IConfigurationService configService)
        {
            _configService = configService;
        }

        public DatabaseContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configService.GetConfig("PostgresDatabaseConnectionString");
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccessToken>(entity =>
            {
                entity.ToTable("accesstoken");
                entity.HasKey(e => e.Id).HasName("PK_AccessToken");
            });

            modelBuilder.Entity<DiscordPlayer>(entity =>
            {
                entity.ToTable("discordplayers");
                entity.HasKey(e => e.PlayerId).HasName("PK_DiscordPlayers");
            });

            modelBuilder.Entity<GameSaves>(entity =>
            {
                entity.ToTable("gamesaves");
                entity.HasKey(a => new { a.PlayerGameSaveId, a.PlayerId });
                entity.Property(b => b.CreatedDate)
                    .HasDefaultValueSql("getdate()");
            });


            modelBuilder.Entity<GameTestingEndState>(entity =>
            {
                entity.ToTable("gametesting_endstate");
                entity.HasKey(e => e.Id).HasName("PK_GameTesting_EndState");
            });

            modelBuilder.Entity<GameTestingError>(entity =>
            {
                entity.ToTable("gametesting_error");
                entity.HasKey(e => e.Id).HasName("PK_GameTesting_Error");
            });

            modelBuilder.Entity<GameTestingGameSave>(entity =>
            {
                entity.ToTable("gametesting_gamesave");
                entity.HasKey(e => e.Id).HasName("PK_GameTesting_GameSave");
            });

            modelBuilder.Entity<GameTestingGameSaveData>(entity =>
            {
                entity.ToTable("gametesting_gamesavedata");
                entity.HasKey(e => e.Id).HasName("PK_GameTesting_GameSaveData");
            });

            modelBuilder.Entity<GameTestingGrammar>(entity =>
            {
                entity.ToTable("gametesting_grammar");
                entity.HasKey(e => e.Id).HasName("PK_GameTesting_Grammar");
            });

            modelBuilder.Entity<GameTestingMiscData>(entity =>
            {
                entity.ToTable("gametesting_miscdata");
                entity.HasKey(e => e.Id).HasName("PK_GameTesting_MiscData");
            });

            modelBuilder.Entity<GameTestingOptionVisited>(entity =>
            {
                entity.ToTable("gametesting_optionvisited");
                entity.HasKey(e => e.Id).HasName("PK_GameTesting_OptionVisited");
            });

            modelBuilder.Entity<GameTestingStateVisited>(entity =>
            {
                entity.ToTable("gametesting_statevisited");
                entity.HasKey(e => e.Id).HasName("PK_GameTesting_StateVisited");
            });

            modelBuilder.Entity<GameTestingWarning>(entity =>
            {
                entity.ToTable("gametesting_warning");
                entity.HasKey(e => e.Id).HasName("PK_GameTesting_Warning");
            });

            modelBuilder.Entity<KikPlayer>(entity =>
            {
                entity.ToTable("kikplayers");
                entity.HasKey(e => e.PlayerId).HasName("PK_KikPlayers");
            });

            modelBuilder.Entity<PlayerAction>(entity =>
            {
                entity.ToTable("playeractions");
                entity.HasKey(e => e.Id).HasName("PK_PlayerActions");
            });

            modelBuilder.Entity<PlayerFrameStack>(entity =>
            {
                entity.ToTable("playerframestack");
                entity.HasKey(e => e.Id).HasName("PK_PlayerFrameStack");
            });

            modelBuilder.Entity<PlayerGameSave>(entity =>
            {
                entity.ToTable("playergamesave");
                entity.HasKey(e => e.SaveId).HasName("PK_PlayerGameSave");
            });

            modelBuilder.Entity<PlayerGameSaveData>(entity =>
            {
                entity.ToTable("playergamesavedata");
                entity.HasKey(e => e.Id).HasName("PK_PlayerGameSaveData");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("players");
                entity.HasKey(e => e.PlayerId).HasName("PK_Players");
            });

            modelBuilder.Entity<PlayerSavedData>(entity =>
            {
                entity.ToTable("playersaveddata");
                entity.HasKey(e => e.Id).HasName("PK_PlayerSavedData");
            });

            modelBuilder.Entity<StatisticsGameAchievement>(entity =>
            {
                entity.ToTable("statisticsgameachievements");
                entity.HasKey(e => e.Id).HasName("PK_StatisticsGameAchievements");
            });

            modelBuilder.Entity<StatisticsGamesByPlayerCount>(entity =>
            {
                entity.ToTable("statisticsgamesbyplayercount");
                entity.HasKey(e => e.Id).HasName("PK_StatisticsGamesByPlayerCount");
            });

            modelBuilder.Entity<StatisticsPosition>(entity =>
            {
                entity.ToTable("statisticspositions");
                entity.HasKey(e => e.Id).HasName("PK_StatisticsPositions");
            });

            modelBuilder.Entity<TelegramPlayer>(entity =>
            {
                entity.ToTable("telegramplayers");
                entity.HasKey(e => e.PlayerId).HasName("PK_TelegramPlayers");
            });

            modelBuilder.Entity<WebPlayer>(entity =>
            {
                entity.ToTable("webplayers");
                entity.HasKey(e => e.PlayerId).HasName("PK_WebPlayers");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
