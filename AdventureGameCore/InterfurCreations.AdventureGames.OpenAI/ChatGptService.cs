using InterfurCreations.AdventureGames.Services.Interfaces;
using OpenAI.GPT3.ObjectModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InterfurCreations.AdventureGames.OpenAI
{
    public class ChatGptService : IAITextService
    {
        private readonly IOpenAIConnector _openAIConnector;
        private Dictionary<string, List<ChatMessage>> _chatDictionaries;

        public ChatGptService(IOpenAIConnector openAIConnector)
        {
            _openAIConnector = openAIConnector;
            _chatDictionaries = new Dictionary<string, List<ChatMessage>>();
        }

        public int GetUserMessageCount(string userId)
        {
            if (_chatDictionaries.TryGetValue(userId, out List<ChatMessage> messages))
            {
                return messages.Count;
            }
            else return 0;
        }

        public List<string> GetUserMessages(string userId)
        {
            if (_chatDictionaries.TryGetValue(userId, out List<ChatMessage> messages))
            {
                return messages.Select(a => a.Content).ToList();
            }
            else return new List<string>();
        }

        public void AddSystemMessage(string userId, string message)
        {
            if (_chatDictionaries.TryGetValue(userId, out List<ChatMessage> messages))
            {
                messages.Add(new ChatMessage("system", message));
            }
            else
            {
                messages = new List<ChatMessage>();
                messages.Add(new ChatMessage("system", message));
                _chatDictionaries.Add(userId, messages);
            }
        }

        public void SeedAssistantMessage(string userId, string message)
        {
            if (_chatDictionaries.TryGetValue(userId, out List<ChatMessage> messages))
            {
                messages.Add(new ChatMessage("assistant", message));
            }
            else
            {
                messages = new List<ChatMessage>();
                messages.Add(new ChatMessage("assistant", message));
                _chatDictionaries.Add(userId, messages);
            }
        }

        public string AddSystemMessageWithResponse(string userId, string message)
        {
            AddSystemMessage(userId, message);
            var messages = _chatDictionaries[userId];
            var result = _openAIConnector.AsyncSendTextForCompletion(messages).Result;
            messages.Add(new ChatMessage("assistant", result));
            return result;
        }

        public string SendMessage(string userId, string message)
        {
            if(_chatDictionaries.TryGetValue(userId, out List<ChatMessage> messages)) 
            {
                messages.Add(new ChatMessage("user", message));
                var result = _openAIConnector.AsyncSendTextForCompletion(messages).Result;
                if(result != null)
                    messages.Add(new ChatMessage("assistant", result));
                return result;
            } else
            {
                messages = new List<ChatMessage>();
                messages.Add(new ChatMessage("user", message));
                var result = _openAIConnector.AsyncSendTextForCompletion(messages).Result;
                messages.Add(new ChatMessage("assistant", result ?? ""));
                _chatDictionaries.Add(userId, messages);
                return result;
            }
        }
        
        public void ClearMessagesForUser(string userId)
        {
            _chatDictionaries.Remove(userId);
        }
    }
}
