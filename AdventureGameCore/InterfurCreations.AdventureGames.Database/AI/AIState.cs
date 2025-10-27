using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Database.AI
{
    public class AIState
    {
        public string Instruction { get; set; }
        public List<(string entity, string message)> Conversation { get; set; }
        public List<string> CurrentOptions { get; set; }
        public bool IsDelete { get; set; }
    }
}
