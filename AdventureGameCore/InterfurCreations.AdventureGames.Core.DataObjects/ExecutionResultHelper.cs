using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Core.DataObjects
{
    public static class ExecutionResultHelper
    {
        public static ExecutionResult SingleMessage(string message, List<string> options, bool askForInput = false)
        {
            return new ExecutionResult
            {
                MessagesToShow = new List<MessageResult>
                {
                    new MessageResult
                    {
                        Message = message
                    }
                },
                OptionsToShow = options,
                AskForInput = askForInput
            };
        }
    }
}
