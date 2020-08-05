using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Core.DataObjects
{
    public class ExecutionResult
    {
        public ExecutionResult()
        {
            OptionsToShow = new List<string>();
            MessagesToShow = new List<MessageResult>();
        }

        public List<string> OptionsToShow { get; set; }
        public List<MessageResult> MessagesToShow { get; set; }
        public bool IsInvalidInput { get; set; }
        public bool AskForInput { get; set; }
        public List<string> StatesVisited { get; set; }
    }
}
