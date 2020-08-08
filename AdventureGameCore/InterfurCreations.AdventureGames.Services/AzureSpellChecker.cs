using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Microsoft.Azure.CognitiveServices.Language.SpellCheck;
using Microsoft.Azure.CognitiveServices.Language.SpellCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services
{
    public class AzureSpellChecker : ISpellChecker
    {
        private readonly SpellCheckClient _spellChecker;
        private readonly IConfigurationService _configService;
        private Dictionary<string, SpellCheckResult> _checkes;

        public AzureSpellChecker(IConfigurationService configService)
        {
            _configService = configService;
            _checkes = new Dictionary<string, SpellCheckResult>();

            _spellChecker = new SpellCheckClient(new ApiKeyServiceClientCredentials(_configService.GetConfig("AzureSpellCheckerKey")));
        }

        public SpellCheckResult CheckSpelling(string text)
        {
            throw new NotImplementedException();
        }

        public async Task<SpellCheckResult> CheckSpellingAsync(string text)
        {
            if (_checkes.TryGetValue(text, out var existingCheck))
                return existingCheck;

            var result = await _spellChecker.SpellCheckerWithHttpMessagesAsync(text: text, mode: "proof");

            var spellCheckResult = ResultFromFlags(text, result.Body.FlaggedTokens);

            _checkes.Add(text, spellCheckResult);

            return spellCheckResult;
        }

        private SpellCheckResult ResultFromFlags(string text, IList<SpellingFlaggedToken> flaggedTokens)
        {
            var result = new SpellCheckResult();
            foreach (var token in flaggedTokens)
            {
                result.suggestions.Add(new SpellCheckResultToken
                {
                    original = token.Token,
                    suggestion = token.Suggestions.FirstOrDefault().Suggestion,
                    wordLocation = token.Offset
                });
            }

            return result;
        }
    }
}
