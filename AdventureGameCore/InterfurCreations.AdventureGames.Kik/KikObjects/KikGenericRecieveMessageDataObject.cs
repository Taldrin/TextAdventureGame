using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik.KikObjects
{
    public class KikGenericRecieveMessageDataObject
    {
        public string type { get; set; }
        public string chatId { get; set; }
        public string from { get; set; }
        public string id { get; set; }
        public List<string> participants { get; set; }
        public long timestamp { get; set; }
        public string chatType { get; set; }
        public bool readReceiptRequested { get; set; }
        public string mention { get; set; }
        public string metadata { get; set; }
    }
}
