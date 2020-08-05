using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Database
{
    public class PlayerGameSave
    {
        public PlayerGameSave()
        {
            GameSaveData = new List<PlayerGameSaveData>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaveId { get; set; }
        public string GameName { get; set; }
        public string StateId { get; set; }
        public string SaveName { get; set; }
        public List<PlayerGameSaveData> GameSaveData { get; set; }
    }
}
