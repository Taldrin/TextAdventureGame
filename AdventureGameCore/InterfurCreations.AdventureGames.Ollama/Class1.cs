using InterfurCreations.AdventureGames.Services.Interfaces;

namespace InterfurCreations.AdventureGames.Ollama
{
    public class OllamaLocalAITextService : IAITextService
    {
        public void AddSystemMessage(string userId, string message)
        {
            throw new NotImplementedException();
        }

        public string AddSystemMessageWithResponse(string userId, string message)
        {
            throw new NotImplementedException();
        }

        public void ClearMessagesForUser(string userId)
        {
            throw new NotImplementedException();
        }

        public int GetUserMessageCount(string userId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetUserMessages(string userId)
        {
            throw new NotImplementedException();
        }

        public void SeedAssistantMessage(string userId, string message)
        {
            throw new NotImplementedException();
        }

        public string SendMessage(string userId, string message)
        {
            throw new NotImplementedException();
        }
    }
}
