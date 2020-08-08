using InterfurCreations.AdventureGames.Services.Interfaces;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services
{
    // Requirement for MSWord to be install makes this difficult to deploy and install on remote systems
    public class MSWordSpellChecker : ISpellChecker
    {
        //private readonly Application _wordApp;
        //private readonly Language _language;
        const string custDict = "custom.dic";

        public MSWordSpellChecker()
        {
            //_wordApp = new Application();
            //_wordApp.Documents.Add();
            //_language = _wordApp.Languages[WdLanguageID.wdEnglishUK];
            //_wordApp.Options.CheckGrammarWithSpelling = true;
        }

        public SpellCheckResult CheckSpellingAsync(string text)
        {
            //var suggestions = _wordApp.GetSpellingSuggestions(text, custDict, MainDictionary: _language);

            return new SpellCheckResult();
        }

        public SpellCheckResult CheckSpelling(string text)
        {
            throw new System.NotImplementedException();
        }

        Task<SpellCheckResult> ISpellChecker.CheckSpellingAsync(string text)
        {
            throw new System.NotImplementedException();
        }
    }
}
