using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Exceptions
{
    public class AdventureGameException : Exception
    {
        public bool ShouldReset { get; private set; }

        public AdventureGameException(string message, bool shouldReset = false) : base(message)
        {
            ShouldReset = shouldReset;
        }
    }
}
