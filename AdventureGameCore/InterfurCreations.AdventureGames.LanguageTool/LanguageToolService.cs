using InterfurCreations.AdventureGames.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.LanguageTool
{
    public class LanguageToolService
    {
        private readonly LanguageToolCommunicator _communicator;

        public LanguageToolService(IConfigurationService configService)
        {
            _communicator = new LanguageToolCommunicator(configService.GetConfig("LanguageToolUrl"));
        }

        public async Task<LanguageToolResponseDataObject> CheckTextAsync(string text)
        {
            var response = await _communicator.PostRequest<LanguageToolResponseDataObject>("check", 
                new Dictionary<string, string> 
                { 
                    { "text", text },
                    { "language", "en-GB" },
                });
            return response;
        }
    }
}
