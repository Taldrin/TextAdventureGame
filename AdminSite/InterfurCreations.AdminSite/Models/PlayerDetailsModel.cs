using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.Models
{
    public class PlayerDetailsModel
    {
        public PlayerDetailsModel()
        {
            recentActions = new List<PlayerActionModel>();
            gameSaves = new List<GameSaveItemModel>();
        }
        public string name { get; set; }
        public string platform { get; set; }
        public int actionCount { get; set; }
        public string id { get; set; }
        public DateTime lastAction { get; set; }
        public List<PlayerActionModel> recentActions { get; set; }
        public List<GameSaveItemModel> gameSaves { get; set; }
    }
}
