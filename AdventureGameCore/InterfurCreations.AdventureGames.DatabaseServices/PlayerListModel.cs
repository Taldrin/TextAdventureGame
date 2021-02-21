using InterfurCreations.AdventureGames.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.DatabaseServices
{
    public class PlayerListModel
    {
        public DiscordPlayer DiscordPlayer { get; set; }
        public TelegramPlayer TelegramPlayer { get; set; }
        public WebPlayer WebPlayer { get; set; }
        public KikPlayer KikPlayer { get; set; }
        public PlayerAction LastAction { get; set; }
        public int ActionCount { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
