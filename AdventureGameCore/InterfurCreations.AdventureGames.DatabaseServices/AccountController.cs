using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.DatabaseServices
{
    public class AccountController : IAccountController
    {
        DatabaseContext _context;

        public AccountController(IDatabaseContextProvider contextProvider)
        {
            _context = contextProvider.GetContext();
        }

        #region Telegram
        public Player CreateNewTelegramAccount(long chatId, string name)
        {
            var newPlayer = _context.Players.Add(new Player { Name = name });
            _context.TelegramPlayers.Add(new TelegramPlayer { ChatId = chatId, PlayerId = newPlayer.Entity.PlayerId });
            _context.SaveChanges();
            return newPlayer.Entity;
        }

        public Player GetOrCreateNewTelegramAccount(long chatId, string name)
        {
            var existing = _context.TelegramPlayers.Where(a => a.ChatId == chatId).Include(a => a.Player).ToList();
            if(existing?.Count == 0)
            {
                return CreateNewTelegramAccount(chatId, name);
            }
            return existing.FirstOrDefault().Player;
        }
        #endregion

        #region Kik
        public Player CreateNewKikAccount(string chatId, string name)
        {
            var newPlayer = _context.Players.Add(new Player { Name = name });
            _context.KikPlayers.Add(new KikPlayer { ChatId = chatId, PlayerId = newPlayer.Entity.PlayerId });
            _context.SaveChanges();
            return newPlayer.Entity;
        }

        public Player GetOrCreateNewKikAccount(string chatId, string name)
        {
            var existing = _context.KikPlayers.Where(a => a.ChatId == chatId).Include(a => a.Player).ToList();
            if (existing?.Count == 0)
            {
                return CreateNewKikAccount(chatId, name);
            }
            return existing.FirstOrDefault().Player;
        }
        #endregion

        #region Discord
        public Player GetOrCreateNewDiscordAccount(long chatId, string name)
        {
            var existing = _context.DiscordPlayers.Where(a => a.ChatId == chatId).Include(a => a.Player).ToList();
            if (existing?.Count == 0)
            {
                return CreateNewDiscordAccount(chatId, name);
            }
            return existing.FirstOrDefault().Player;
        }

        private Player CreateNewDiscordAccount(long chatId, string name)
        {
            var newPlayer = _context.Players.Add(new Player { Name = name });
            _context.DiscordPlayers.Add(new DiscordPlayer { ChatId = chatId, PlayerId = newPlayer.Entity.PlayerId });
            _context.SaveChanges();
            return newPlayer.Entity;
        }
        #endregion

        #region Web
        public Player GetOrCreateNewWebAccount(string accessKey, string name)
        {
            var existing = _context.WebPlayers.Where(a => a.AccessKey == accessKey).Include(a => a.Player).ToList();
            if (existing?.Count == 0)
            {
                return CreateNewWebAccount(accessKey, name);
            }
            return existing.FirstOrDefault().Player;
        }

        public Player CreateNewWebAccount(string accessKey, string name)
        {
            var newPlayer = _context.Players.Add(new Player { Name = name });
            _context.WebPlayers.Add(new WebPlayer { AccessKey = accessKey, PlayerId = newPlayer.Entity.PlayerId });
            _context.SaveChanges();
            return newPlayer.Entity;
        }
        #endregion
    }
}
