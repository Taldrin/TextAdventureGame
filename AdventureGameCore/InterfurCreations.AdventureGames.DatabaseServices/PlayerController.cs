using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.DatabaseServices
{
    public class PlayerDatabaseController : IPlayerDatabaseController
    {
        DatabaseContext _context;

        private Dictionary<long, (DateTime LastRetrieved, Player DatabaseObject)> TelegramPlayerCache = new Dictionary<long, (DateTime LastRetrieved, Player DatabaseObject)>();
        private Dictionary<long, (DateTime LastRetrieved, Player DatabaseObject)> DiscordPlayerCache = new Dictionary<long, (DateTime LastRetrieved, Player DatabaseObject)>();
        private Dictionary<string, (DateTime LastRetrieved, Player DatabaseObject)> KikPlayerCache = new Dictionary<string, (DateTime LastRetrieved, Player DatabaseObject)>();
        private Dictionary<string, (DateTime LastRetrieved, Player DatabaseObject)> WebPlayerCache = new Dictionary<string, (DateTime LastRetrieved, Player DatabaseObject)>();

        private readonly TimeSpan CacheTime = TimeSpan.FromMinutes(10);

        public PlayerDatabaseController(IDatabaseContextProvider contextProvider)
        {
            _context = contextProvider.GetContext();
        }

        public Player GetPlayerById(string id)
        {
            return _context.Players.Where(a => a.PlayerId == id).LoadPlayerData().Include(a => a.TelegramPlayer).Include(a => a.DiscordPlayer).Include(a => a.KikPlayer).Include(a => a.WebPlayer).SingleOrDefault();
        }

        public List<PlayerAction> GetPlayerActions(string playerId, int maxActions = 50)
        {
            return _context.PlayerActions.Where(a => a.PlayerId == playerId).OrderByDescending(a => a.Time).Take(maxActions).ToList();
        }

        public List<GameSaves> GetPlayerGameSaves(string playerId, int maxSaves = 50)
        {
            return _context.GameSaves.Include(a => a.PlayerGameSave).ThenInclude(a => a.GameSaveData).Where(a => a.PlayerId == playerId).OrderByDescending(a => a.CreatedDate).Take(maxSaves).ToList();
        }

        public (List<PlayerListModel> Players, int PageCount) ListPlayers(PlatformType platform, string playerName, int pageNumber, int pageSize)
        {
            IQueryable<Player> query = null;
            if (platform == PlatformType.Telegram)
                query = _context.Players.Where(a => a.TelegramPlayer != null);
            else if (platform == PlatformType.Discord)
                query = _context.Players.Where(a => a.DiscordPlayer != null);
            else if (platform == PlatformType.Kik)
                query = _context.Players.Where(a => a.KikPlayer != null);
            else if (platform == PlatformType.Web)
                query = _context.Players.Where(a => a.WebPlayer != null);
            else
                query = _context.Players;

            if (!string.IsNullOrEmpty(playerName))
                query = query.Where(a => a.Name.Contains(playerName));

            var results = query.OrderByDescending(a => a.Actions.Max(b => (DateTime?)b.Time)).Skip(pageNumber * pageSize).Include(a => a.DiscordPlayer).
                Include(a => a.KikPlayer).Include(a => a.TelegramPlayer).Include(a => a.WebPlayer).Select(a => new PlayerListModel
                {  
                    DiscordPlayer = a.DiscordPlayer,
                    LastAction = a.Actions.OrderByDescending(a => a.Time).FirstOrDefault(),
                    ActionCount = a.Actions.Count,
                    Name = a.Name,
                    KikPlayer = a.KikPlayer,
                    WebPlayer = a.WebPlayer,
                    TelegramPlayer = a.TelegramPlayer,
                    Id = a.PlayerId
                }).Take(pageSize).ToList();

            var totalCount = query.Count();
            return (results, totalCount);
        }

        public Player GetPlayerByTelegramChannel(long chatId)
        {
            if (TelegramPlayerCache.TryGetValue(chatId, out var cacheValue))
            {
                if (cacheValue.LastRetrieved > DateTime.Now.Subtract(CacheTime))
                {
                    return cacheValue.DatabaseObject;
                }
                else
                    TelegramPlayerCache.Remove(chatId);
            }
            Player telegramPlayer = null;
            var player = _context.TelegramPlayers.Include(a => a.Player).ThenInclude(a => a.ActiveGameSave).ThenInclude(a => a.GameSaveData).Include(a => a.Player)
                .ThenInclude(a => a.AccessTokens).Include(a => a.Player).ThenInclude(a => a.PermanentData).Include(a => a.Player).ThenInclude(a => a.ActiveGameSave).ThenInclude(a => a.FrameStack).FirstOrDefault(a => a.ChatId == chatId);

            if (player != null)
            {
                var gameSaves = GetGameSaves(player.PlayerId);
                telegramPlayer = player.Player;
                telegramPlayer.GameSaves = gameSaves;
            }
            if (telegramPlayer == null) return null;
            TelegramPlayerCache.Add(chatId, (DateTime.Now, telegramPlayer));
            return telegramPlayer;
        }

        public List<GameSaves> GetGameSaves(string playerId)
        {
            var gameSaves = _context.GameSaves.Where(a => a.PlayerId == playerId)
                .Include(a => a.PlayerGameSave).ThenInclude(a => a.GameSaveData).OrderByDescending(a => a.CreatedDate).Take(10).ToList();

            return gameSaves;
        }

        public GameSaves GetSaveById(int saveId, string playerId)
        {
            return _context.GameSaves.Where(a => a.PlayerGameSaveId == saveId && a.PlayerId == playerId).Include(a => a.PlayerGameSave).ThenInclude(a => a.GameSaveData).SingleOrDefault();
        }
        
        public Player GetPlayerByKikChannel(string chatId)
        {
            if (KikPlayerCache.TryGetValue(chatId, out var cacheValue))
            {
                if (cacheValue.LastRetrieved > DateTime.Now.Subtract(CacheTime))
                {
                    return cacheValue.DatabaseObject;
                }
                else
                    KikPlayerCache.Remove(chatId);
            }

            var kikPlayer = _context.KikPlayers.Where(a => a.ChatId == chatId).Select(a => a.Player).LoadPlayerData().SingleOrDefault();
            if (kikPlayer == null) return null;
            KikPlayerCache.Add(chatId, (DateTime.Now, kikPlayer));
            return kikPlayer;
        }

        public Player GetPlayerByDiscordAuthor(long authorId)
        {
            if (DiscordPlayerCache.TryGetValue(authorId, out var cacheValue))
            {
                if (cacheValue.LastRetrieved > DateTime.Now.Subtract(CacheTime))
                {
                    return cacheValue.DatabaseObject;
                }
                else
                    DiscordPlayerCache.Remove(authorId);
            }

            Player discordPlayer = null;
            var player = _context.DiscordPlayers.Include(a => a.Player).ThenInclude(a => a.ActiveGameSave).ThenInclude(a => a.GameSaveData).Include(a => a.Player)
                .ThenInclude(a => a.AccessTokens).Include(a => a.Player).ThenInclude(a => a.PermanentData).Include(a => a.Player).ThenInclude(a => a.ActiveGameSave).ThenInclude(a => a.FrameStack).FirstOrDefault(a => a.ChatId == authorId);

            if (player != null)
            {
                var gameSaves = GetGameSaves(player.PlayerId);
                discordPlayer = player.Player;
                discordPlayer.GameSaves = gameSaves;
            }

            if (discordPlayer == null) return null;
            DiscordPlayerCache.Add(authorId, (DateTime.Now, discordPlayer));
            return discordPlayer;
        }

        public Player GetPlayerByWebKey(string webKey)
        {
            if(WebPlayerCache.TryGetValue(webKey, out var cacheValue)) {
                if (cacheValue.LastRetrieved > DateTime.Now.Subtract(CacheTime))
                {
                    return cacheValue.DatabaseObject;
                } else
                    WebPlayerCache.Remove(webKey);
            }

            Player webPlayer = null;
            var player = _context.WebPlayers.Include(a => a.Player).ThenInclude(a => a.ActiveGameSave).ThenInclude(a => a.GameSaveData).Include(a => a.Player)
                .ThenInclude(a => a.AccessTokens).Include(a => a.Player).ThenInclude(a => a.PermanentData).Include(a => a.Player).ThenInclude(a => a.ActiveGameSave).ThenInclude(a => a.FrameStack).FirstOrDefault(a => a.AccessKey == webKey);

            if (player != null)
            {
                var gameSaves = _context.GameSaves.Where(a => a.PlayerId == player.PlayerId)
                    .Include(a => a.PlayerGameSave).ThenInclude(a => a.GameSaveData).OrderByDescending(a => a.CreatedDate).Take(10).ToList();
                webPlayer = player.Player;
                webPlayer.GameSaves = gameSaves;
            }
            if (webPlayer == null) return null;

            if (webPlayer == null) throw new Exception("Could not found player with key: " + webKey);
            WebPlayerCache.Add(webKey, (DateTime.Now, webPlayer));
            return webPlayer;
        }
    }
}
