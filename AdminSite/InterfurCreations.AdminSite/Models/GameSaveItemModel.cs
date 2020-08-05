using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.Models
{
    public class GameSaveItemModel
    {
        public string gameName { get; set; }
        public int saveId { get; set; }
        public DateTime dateCreated { get; set; }
    }
}
