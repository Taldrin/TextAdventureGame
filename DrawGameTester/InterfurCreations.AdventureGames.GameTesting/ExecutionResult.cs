using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.GameTesting
{
    public class ExecutionResult
    {
        public List<MessageResult> MessagesReceived { get; set; }
        public List<Exception> ErrorsReceived { get; set; }
        public List<string> OptionsReceived { get; set; }
        public PlayerState NewState { get; set; }
        public bool IsErrored { get; set; }
    }
}
