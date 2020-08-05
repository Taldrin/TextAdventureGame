using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.Services.Interfaces
{
    public interface IAccessService
    {
        bool DoesPlayerHaveAccess(Database.Player player);
        (bool result, string reason) TryGrantAccess(Database.Player player, string accessToken);
    }
}
