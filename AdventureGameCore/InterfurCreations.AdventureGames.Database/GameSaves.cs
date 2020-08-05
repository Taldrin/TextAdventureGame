using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Database
{
    public class GameSaves
    {
        [ForeignKey("PlayerGameSaveId")]
        public int PlayerGameSaveId { get; set; }
        [ForeignKey("Player")]
        public string PlayerId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public PlayerGameSave PlayerGameSave { get; set; }
        public Player Player { get; set; }
    }
}
