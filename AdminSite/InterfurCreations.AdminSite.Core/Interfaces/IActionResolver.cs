using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdminSite.Core.Interfaces
{
    public interface IActionResolver
    {
        DataObjects.ActionDetailsDataObject ResolveAction(AdventureGames.Database.PlayerAction action, TimeSpan? liveGameTimeBetweenCheck = null);
    }
}
