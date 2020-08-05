using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Database
{
    public class PlayerAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Player")]
        public string PlayerId { get; set; }
        public DateTime Time { get; set; }
        public string ActionName { get; set; }
        public string GameName{ get; set; }
        public Player Player { get; set; }
    }
}
