using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.Services.Interfaces
{
    public interface IPlayerActionReporter
    {
        void ReportAction(Database.Player player, string actionName, string gameName);
    }
}
