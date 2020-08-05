using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InterfurCreations.AdventureGames.Database
{
    public class AccessToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Player")]
        public string PlayerId { get; set; }
        public DateTime LastActivated { get; set; }
        public string Token { get; set; }
        public Player Player { get; set; }
        public int HoursForRefresh { get; set; }
        public string TokenType { get; set; }
    }
}
