using InterfurCreations.AdventureGames.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfurCreations.AdventureGames.DatabaseServices
{
    public static class DatabaseQueryHelper
    {
        public static IQueryable<Player> LoadPlayerData(this IQueryable<Player> query)
        {
            return query.Include(a => a.GameSaves).ThenInclude(a => a.PlayerGameSave).ThenInclude(a => a.GameSaveData).Include(a => a.ActiveGameSave)
                .ThenInclude(a => a.GameSaveData).Include(a => a.AccessTokens).Include(a => a.PermanentData);
        }
    }
}
