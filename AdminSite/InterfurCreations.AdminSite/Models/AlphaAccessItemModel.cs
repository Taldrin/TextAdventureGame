using InterfurCreations.AdventureGames.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.Models
{
    public class AlphaAccessItemModel
    {
        public string AccessCode { get; set; }
        public int Id { get; set; }
        public DateTime LastActivated { get; set; }
        public string PlayerName { get; set; }
        public string PlayerPlatform { get; set; }
        public int HoursAllowed { get; set; }
    }
}
