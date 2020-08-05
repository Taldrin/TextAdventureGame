using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.Core.DataObjects
{
    public class OptionResult
    {
        // OptionType can be safely ignored, but it can be used to create better UIs, by knowing what a button is for.
        public HintOptionType Hint { get; set; }
        public string Option { get; set; }

        public static explicit operator string (OptionResult result)
        {
            return result.Option;
        }
    }
}
