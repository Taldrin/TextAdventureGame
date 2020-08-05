using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik.KikObjects
{
    public class KikMessagesDataObject<T> where T : KikGenericRecieveMessageDataObject
    {
        public List<T> messages { get; set; }
    }
}
