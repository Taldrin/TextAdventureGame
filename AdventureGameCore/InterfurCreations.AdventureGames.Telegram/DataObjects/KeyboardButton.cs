using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Telegram.DataObjects
{
    public class KeyboardButton
    {
        public string text { get; set; }
        public string callback_data { get => text; }
    }
}
