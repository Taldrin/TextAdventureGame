using InterfurCreations.AdventureGames.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.GameLanguage
{
    public class ParsedStateOption
    {
        public bool IsDirectTransition;
        public bool DirectTransitionCommandResult;
        public OptionType OptionType;
        public string text;
        public DrawState resultState;
        public string Id;
    }
}
