using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Telegram.DataObjects
{
    public class ReplyKeyboardMarkup
    {
        public List<List<KeyboardButton>> keyboard { get; set; }
        public bool resize_keyboard { get => true; }
    }
}
