using OpenAI.GPT3.ObjectModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.OpenAI
{
    public interface IOpenAIConnector
    {
        Task<string?> AsyncSendTextForCompletion(List<ChatMessage> chatSoFar);
    }
}
