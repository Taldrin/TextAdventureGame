using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services.Interfaces
{
    public interface IAITextService
    {
        void AddSystemMessage(string userId, string message);
        string AddSystemMessageWithResponse(string userId, string message);
        int GetUserMessageCount(string userId);
        string SendMessage(string userId, string message);
    }
}
