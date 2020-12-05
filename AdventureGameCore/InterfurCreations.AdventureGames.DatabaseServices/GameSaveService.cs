using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
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

            var addedSave = _context.PlayerGameSave.Add(gameSave);
            addedSave.Entity.FrameStack = player.ActiveGameSave.FrameStack.Select(a => new PlayerFrameStack
            {
                CreatedDate = a.CreatedDate,
                FunctionName = a.FunctionName,
                ReturnStateId = a.ReturnStateId,
                Save = addedSave.Entity
            }).ToList();

            player.GameSaves.Add(new GameSaves { PlayerGameSave = addedSave.Entity, Name = ""});
        }

        public GameSaves GetGameSaveById(int saveId, string playerId)
        {
            return _context.GameSaves.Include(a => a.PlayerGameSave).ThenInclude(a => a.FrameStack).SingleOrDefault(a => a.PlayerGameSaveId == saveId && a.PlayerId == playerId);
        }
    }
}
