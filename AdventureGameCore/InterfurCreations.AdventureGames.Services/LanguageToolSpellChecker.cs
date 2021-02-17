using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.LanguageTool;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var result = new SpellCheckResult();
            result.sourceText = text;
            result.suggestions = new List<SpellCheckResultToken>();

            var response = await _languageToolService.CheckTextAsync(text);
            foreach(var match in response.matches)
            {
                var token = new SpellCheckResultToken();
                token.original = match.context.text;
                token.suggestion = match.replacements.FirstOrDefault()?.value ?? "N/A";
                token.wordLocation = match.offset;
                token.message = match.message;
                result.suggestions.Add(token);
            } 
            return result;
        }
    }
}
