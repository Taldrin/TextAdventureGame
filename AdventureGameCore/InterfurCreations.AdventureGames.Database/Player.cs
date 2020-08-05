using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterfurCreations.AdventureGames.Database
{
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string PlayerId { get; set; }
        public string Name { get; set; }
        public string PlayerFlag { get; set; }
        public string PlayerMenuContext { get; set; }
        public string PlayerMenuContext2 { get; set; }
        [ForeignKey("PlayerGameSave")]
        public int? ActiveGameSaveId { get; set; }
        public PlayerGameSave ActiveGameSave { get; set; }
        public List<PlayerAction> Actions { get; set; }
        public List<GameSaves> GameSaves { get; set; }
        public List<AccessToken> AccessTokens { get; set; }
        public List<PlayerSavedData> PermanentData { get; set; }
        public DiscordPlayer DiscordPlayer { get; set; }
        public TelegramPlayer TelegramPlayer { get; set; }
        public KikPlayer KikPlayer { get; set; }
        public WebPlayer WebPlayer { get; set; }

        public Player()
        {
            GameSaves = new List<GameSaves>();
            AccessTokens = new List<AccessToken>();
            Actions = new List<PlayerAction>();
            PermanentData = new List<PlayerSavedData>();
        }
    }
}
