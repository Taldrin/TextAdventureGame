using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik.KikObjects.Send
{
    public class KikKeyboardDataObject
    {
        public string type { get; set; }
        public List<KikKeyboardKeyDataObject> responses { get; set; }
    }
}
