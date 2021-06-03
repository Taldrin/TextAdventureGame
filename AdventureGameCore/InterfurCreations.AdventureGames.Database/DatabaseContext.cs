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
            //var connectionString = "Server=localhost;Database=AdventureBot;Trusted_Connection=True";
            var connectionString = _configService.GetConfig("DatabaseConnectionString");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameSaves>().HasKey(a => new { a.PlayerGameSaveId, a.PlayerId });
            modelBuilder.Entity<GameSaves>()
                .Property(b => b.CreatedDate)
                .HasDefaultValueSql("getdate()");

            BuildGameTesting(modelBuilder);
        }

        protected void BuildGameTesting(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameTestingEndState>().ToTable("GameTesting_EndState");
            modelBuilder.Entity<GameTestingError>().ToTable("GameTesting_Error");
            modelBuilder.Entity<GameTestingGameSave>().ToTable("GameTesting_GameSave");
            modelBuilder.Entity<GameTestingGameSaveData>().ToTable("GameTesting_GameSaveData");
            modelBuilder.Entity<GameTestingMiscData>().ToTable("GameTesting_MiscData");
            modelBuilder.Entity<GameTestingOptionVisited>().ToTable("GameTesting_OptionVisited");
            modelBuilder.Entity<GameTestingGrammar>().ToTable("GameTesting_Grammar");
            modelBuilder.Entity<GameTestingStateVisited>().ToTable("GameTesting_StateVisited");
            modelBuilder.Entity<GameTestingWarning>().ToTable("GameTesting_Warning");
        }
    }
}
