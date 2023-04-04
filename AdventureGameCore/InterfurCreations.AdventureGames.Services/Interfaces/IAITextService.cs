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
        void ClearMessagesForUser(string userId);
        int GetUserMessageCount(string userId);
        void SeedAssistantMessage(string userId, string message);
        string SendMessage(string userId, string message);
    }
}
