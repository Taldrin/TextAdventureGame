using InterfurCreations.AdventureGames.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.GameTesting
{
    public class UniqueTestDataItemWithSave
    {
        public string Value { get; set; }
        public PlayerGameSave GameSave { get; set; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(obj is UniqueTestDataItemWithSave item)
                return Value.Equals(item.Value);
            return false;
        }

        public static implicit operator UniqueTestDataItemWithSave(string val)
        {
            return new UniqueTestDataItemWithSave { Value = val };
        }
    }
}
