using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.LanguageTool;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services
{
    public class LanguageToolSpellChecker : ISpellChecker
    {
        private LanguageToolService _languageToolService;

        public LanguageToolSpellChecker(IConfigurationService configurationService)
        {
            _languageToolService = new LanguageToolService(configurationService);
        }

        public SpellCheckResult CheckSpelling(string text)
        {
            throw new NotImplementedException();
        }

        public async Task<SpellCheckResult> CheckSpellingAsync(string text)
        {
            var response = await _languageToolService.CheckTextAsync(text);
            // Create user friendly SpellCheckResult from response
            return null;
        }
    }
}
