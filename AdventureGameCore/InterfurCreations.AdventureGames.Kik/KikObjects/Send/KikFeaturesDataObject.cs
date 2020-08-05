using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik.KikObjects.Send
{
    public class KikFeaturesDataObject
    {
        public bool receiveReadReceipts { get; set; }
        public bool receiveIsTyping { get; set; }
        public bool manuallySendReadReceipts { get; set; }
        public bool receiveDeliveryReceipts { get; set; }
    }
}
