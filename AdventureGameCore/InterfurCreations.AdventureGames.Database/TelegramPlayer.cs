using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterfurCreations.AdventureGames.Database
{
    public class TelegramPlayer
    {
        [Key]
        [ForeignKey("Player")]
        public string PlayerId { get; set; }
        public long ChatId { get; set; }
        public Player Player { get; set; }
    }
}
