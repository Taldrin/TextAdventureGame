using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InterfurCreations.AdventureGames.Database
{
    public class WebPlayer
    {
        [Key]
        [ForeignKey("Player")]
        public string PlayerId { get; set; }
        public string AccessKey { get; set; }
        public Player Player { get; set; }
    }
}
