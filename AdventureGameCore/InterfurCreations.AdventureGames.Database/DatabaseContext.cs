using InterfurCreations.AdventureGames.Configuration;
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

        public DatabaseContext(IConfigurationService configService)
        {
            _configService = configService;
        }

        public DatabaseContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString =  _configService.GetConfig("DatabaseConnectionString");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameSaves>().HasKey(a => new { a.PlayerGameSaveId, a.PlayerId });
            modelBuilder.Entity<GameSaves>()
                .Property(b => b.CreatedDate)
                .HasDefaultValueSql("getdate()");
        }
    }
}
