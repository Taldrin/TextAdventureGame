using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services.Interfaces
{
    public interface ISpellChecker
    {
        SpellCheckResult CheckSpelling(string text);
        Task<SpellCheckResult> CheckSpellingAsync(string text);
    }
}
