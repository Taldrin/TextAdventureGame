using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik.KikObjects.Send
{
    public class KikPictureMessageDataObject : KikGenericSendMessageDataObject
    {
        public string picUrl { get; set; }
        public List<KikKeyboardDataObject> keyboards { get; set; }
    }
}
