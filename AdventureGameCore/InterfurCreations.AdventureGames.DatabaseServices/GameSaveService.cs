using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.DatabaseServices
{
    public class GameSaveService : IGameSaveService
    {
        private readonly DatabaseContext _context;

        public GameSaveService(IDatabaseContextProvider contextProvider)
        {
            _context = contextProvider.GetContext();
        }

        public void SaveCurrentGame(Player player)
        {
            PlayerGameSave gameSave = new PlayerGameSave
            {
                GameName = player.ActiveGameSave.GameName,
                GameSaveData = player.ActiveGameSave.GameSaveData.Select(a => new PlayerGameSaveData
                {
                    Name = a.Name,
                    Value = a.Value,
                }).ToList(),
                StateId = player.ActiveGameSave.StateId,
            };

            player.GameSaves.Add(new GameSaves { PlayerGameSave = gameSave, Name = ""});
        }

        public GameSaves GetGameSaveById(int saveId, string playerId)
        {
            return _context.GameSaves.SingleOrDefault(a => a.PlayerGameSaveId == saveId && a.PlayerId == playerId);
        }
    }
}
