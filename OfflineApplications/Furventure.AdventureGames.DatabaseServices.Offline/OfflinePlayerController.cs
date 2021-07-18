using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furventure.AdventureGames.DatabaseServices.Offline
{
    public class OfflinePlayerController : IOfflinePlayerController
    {
        private readonly OfflineDatabaseContextProvider _dbProvider;

        public OfflinePlayerController(OfflineDatabaseContextProvider contextProvider)
        {
            _dbProvider = contextProvider;
        }

        public Player GetPlayerByProfile(string name)
        {
            return _dbProvider.GetContext().Players.FirstOrDefault();
        }
    }
}
