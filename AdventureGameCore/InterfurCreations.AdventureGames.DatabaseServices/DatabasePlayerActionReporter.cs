using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.DatabaseServices
{
    public class DatabasePlayerActionReporter : IPlayerActionReporter
    {
        public void ReportAction(Player player, string actionName, string gameName)
        {
            player.Actions.Add(new PlayerAction {ActionName = actionName, GameName = gameName, Player = player, Time = DateTime.UtcNow});
        }
    }
}
