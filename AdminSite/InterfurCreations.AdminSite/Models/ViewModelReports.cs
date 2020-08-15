using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotAdminSite.Models
{
    public class ViewModelReports
    {
        public ViewModelReports()
        {
            ActionsByGame = new Dictionary<string, int>();
        }

        public int ActionsCount { get; set; }
        public int PlayersCount { get; set; }
        public Dictionary<string, int> ActionsByGame { get; set; }
        public int TotalActions { get; set; }
        public int TotalPlayers { get; set; }
        public int TotalGameSaves { get; set; }
    }
}
