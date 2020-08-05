using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.Services
{
    public class SpellCheckResult
    {
        public SpellCheckResult()
        {
            suggestions = new List<SpellCheckResultToken>();
        }

        public string sourceText;
        public List<SpellCheckResultToken> suggestions;
    }

    public class SpellCheckResultToken
    {
        public string original;
        public string suggestion;
        public int wordLocation;
    }
}
