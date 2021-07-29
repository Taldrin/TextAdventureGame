using InterfurCreations.AdventureGames.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furventure.AdventureGames.DatabaseServices.Offline
{
    public interface IOfflinePlayerController
    {
        void CreateNewProfile(string name);
        Player GetPlayerByProfile(string name);
    }
}
