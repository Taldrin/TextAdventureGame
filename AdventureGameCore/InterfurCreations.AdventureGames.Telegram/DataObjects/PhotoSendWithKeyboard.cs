using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Telegram.DataObjects
{
    public class PhotoSendWithKeyboard
    {
        public ReplyKeyboardMarkup reply_markup { get; set; }
        public long chat_id { get; set; }
        public string photo { get; set; }
    }
}
