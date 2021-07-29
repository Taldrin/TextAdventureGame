using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furventure.AdventureGames.DatabaseServices.Offline
{
    public class OfflinePlayerController : IOfflinePlayerController
    {
        private readonly IDatabaseContextProvider _dbProvider;

        public OfflinePlayerController(IDatabaseContextProvider contextProvider)
        {
            _dbProvider = contextProvider;
        }

        public Player GetPlayerByProfile(string name)
        {
            return _dbProvider.GetContext().Players.Include(a => a.ActiveGameSave)
                .ThenInclude(a => a.GameSaveData).Include(a => a.ActiveGameSave).ThenInclude(a => a.FrameStack)
                .Include(a => a.PermanentData).FirstOrDefault();
        }

        public void CreateNewProfile(string name)
        {
            var context = _dbProvider.GetContext();
            context.Players.Add(new Player
            {
                Name = name,
            });
            context.SaveChanges();
        }
    }
}
