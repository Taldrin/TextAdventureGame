using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Telegram.DataObjects
{
    public class MessageSendWithKeyboard : MessageSend
    {
        public ReplyKeyboardMarkup reply_markup { get; set; }
    }
}
