using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.Models
{
    public class AchievementItemViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PercentOfPlayersAchieved { get; set; }
        public long NumberOfPlayersAchieved { get; set; }
    }
}
