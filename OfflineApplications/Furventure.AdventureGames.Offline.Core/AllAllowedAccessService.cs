using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furventure.AdventureGames.Offline.Core
{
    public class AllAllowedAccessService : IAccessService
    {
        public bool DoesPlayerHaveAccess(Player player)
        {
            return true;
        }

        public (bool result, string reason) TryGrantAccess(Player player, string accessToken)
        {
            return (true, "");
        }
    }
}
