using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Core.Interface
{
    public interface IGameExecutor
    {
        List<string> GetPossibleOptionsFromState(PlayerState state);
        DataObjects.ExecutionResult ProcessNewMessage(string message, PlayerState state);
    }
}
