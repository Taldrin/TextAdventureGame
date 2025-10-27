using InterfurCreations.AdventureGames.Configuration;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace InterfurCreations.AdventureGames.OpenAI
{
    public class OpenRouterClientProvider
    {
        private IConfigurationService _configuration;

        public OpenRouterClientProvider(IConfigurationService configuration)
        {
            _configuration = configuration;
        }

        public ChatClient BuildClient()
        {
            ChatClient client = new(
                //model: "thedrummer/cydonia-24b-v4.1",
                model: "mistralai/mistral-large-2411",
                credential: new ApiKeyCredential(_configuration.GetConfig("OpenRouterKey")),
                options: new OpenAIClientOptions()
                {
                    Endpoint = new Uri("https://openrouter.ai/api/v1")
                }
            );

            return client;
        }
    }
}
