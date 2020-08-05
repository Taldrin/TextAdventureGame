using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik.KikObjects.Send
{
    public class KikMessageSendWithKeyboardDataObject : KikGenericSendMessageDataObject
    {
        public string body { get; set; }
        public List<KikKeyboardDataObject> keyboards { get; set; }
    }
}
