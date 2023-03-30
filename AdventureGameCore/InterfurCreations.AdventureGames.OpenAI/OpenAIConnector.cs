using InterfurCreations.AdventureGames.Configuration;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.OpenAI
{
    public class OpenAIConnector : IOpenAIConnector
    {
        private OpenAIService _openAiService;
        private IConfigurationService _configService;

        public OpenAIConnector(IConfigurationService configService)
        {
            _configService = configService;
            _openAiService = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = _configService.GetConfig("OpenAIKey")
            });
        }

        public async Task<string?> AsyncSendTextForCompletion(List<ChatMessage> chatSoFar)
        {
            var completionResult = await _openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest()
            {
                Messages = chatSoFar,
                Model = Models.ChatGpt3_5Turbo
            });


            if (completionResult.Successful)
            {
                if (completionResult.Usage.TotalTokens >= 4000)
                    chatSoFar.RemoveAt(4);
                return completionResult.Choices.FirstOrDefault()?.Message.Content;
            }
            else
            {
                if (completionResult.Error == null)
                {
                    throw new Exception("Unknown Error");
                }
                throw new Exception($"{completionResult.Error.Code}: {completionResult.Error.Message}");
            }
        }
    }
}
