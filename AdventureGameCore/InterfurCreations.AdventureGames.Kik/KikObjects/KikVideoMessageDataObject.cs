using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik.KikObjects
{
    public class KikVideoMessageDataObject : KikGenericRecieveMessageDataObject
    {
        public string videoUrl { get; set; }
        public KikVideoMessageAttributionData attribution { get; set; }
    }

    public class KikVideoMessageAttributionData
    {
        public string name { get; set; }
        public string iconUrl { get; set; }
    }
}
