using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Telegram.DataObjects
{
    public class PhotoSendWithKeyboardAndCaption : PhotoSendWithKeyboard
    {
        public string caption { get; set; }
    }
}
