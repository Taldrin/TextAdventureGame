using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik.KikObjects.Send
{
    public abstract class KikGenericSendMessageDataObject
    {
        public string chatId { get; set; }
        public string type { get; set; }
        public string to { get; set; }
    }
}
