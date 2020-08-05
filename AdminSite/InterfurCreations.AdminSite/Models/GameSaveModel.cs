using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.Models
{
    public class GameSaveModel
    {
        public GameSaveModel()
        {
            saveData = new List<(string name, string value)>();
        }
        public string gameName { get; set; }
        public int saveId { get; set; }
        public DateTime dateCreated { get; set; }
        public List<(string name, string value)> saveData { get; set; }
    }
}
