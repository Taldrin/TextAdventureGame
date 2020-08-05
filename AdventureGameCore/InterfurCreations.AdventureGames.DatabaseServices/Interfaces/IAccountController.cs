using InterfurCreations.AdventureGames.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.DatabaseServices.Interfaces
{
    public interface IAccountController
    {
        Player GetOrCreateNewTelegramAccount(long chatId, string name);
        Player GetOrCreateNewDiscordAccount(long chatId, string name);
        Player CreateNewKikAccount(string chatId, string name);
        Player GetOrCreateNewKikAccount(string chatId, string name);
        Player GetOrCreateNewWebAccount(string accessKey, string name);
        Player CreateNewWebAccount(string accessKey, string name);
    }
}
