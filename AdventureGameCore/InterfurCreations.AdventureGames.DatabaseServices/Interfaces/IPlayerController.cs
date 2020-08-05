using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.DatabaseServices.Interfaces
{
    public interface IPlayerDatabaseController
    {
        Player GetPlayerByDiscordAuthor(long authorId);
        Player GetPlayerById(string id);
        Player GetPlayerByKikChannel(string chatId);
        Player GetPlayerByTelegramChannel(long chatId);
        Player GetPlayerByWebKey(string webKey);
        GameSaves GetSaveById(int saveId, string playerId);
        (List<Player> Players, int PageCount) ListPlayers(PlatformType platform, string playerName, int pageNumber, int pageSize);
    }
}
