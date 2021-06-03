using InterfurCreations.AdventureGames.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services
{
    public static class GameSaveExtensions
    {
        public static PlayerGameSave Clone(this PlayerGameSave gameSave)
        {
            return new PlayerGameSave
            {
                GameName = gameSave.GameName,
                SaveName = gameSave.SaveName,
                GameSaveData = gameSave.GameSaveData.Select(a => new PlayerGameSaveData
                {
                    Name = a.Name,
                    Value = a.Value
                }).ToList(),
                StateId = gameSave.StateId,
                FrameStack = gameSave.FrameStack.Select(a => new PlayerFrameStack
                {
                    CreatedDate = a.CreatedDate,
                    FunctionName = a.FunctionName,
                    ReturnStateId = a.ReturnStateId,
                }).ToList()
            };
        }
    }
}
