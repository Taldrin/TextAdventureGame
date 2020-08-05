using InterfurCreations.AdventureGames.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services.Interfaces
{
    public interface IGameSaveService
    {
        GameSaves GetGameSaveById(int saveId, string playerId);
        void SaveCurrentGame(Player player);
    }
}
