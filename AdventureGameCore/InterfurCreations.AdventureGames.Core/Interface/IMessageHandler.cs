using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Core.Interface
{
    public interface IMessageHandler
    {
        bool ShouldHandleMessage(string message, string gameState, string playerFlag);
        ExecutionResult HandleMessage(string message, Player player);
        List<string> GetOptions(Player player);
        int Priorioty { get; }
    }
}
