using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Database
{
    public class DiscordPlayer
    {
        [Key]
        [ForeignKey("Player")]
        public string PlayerId { get; set; }
        public long ChatId { get; set; }
        public Player Player { get; set; }
    }
}
