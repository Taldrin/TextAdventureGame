using InterfurCreations.AdventureGames.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.Models
{
    public class ViewModelGameDetails
    {
        public string GameName { get; set; }
        public string Id { get; set; }
        public int minutesToRunFor { get; set; }
        public int WordCount { get; set; }
        public int OptionCount { get; set; }
        public int StateCount { get; set; }
        public List<AchievementItemViewModel> Achievements { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
