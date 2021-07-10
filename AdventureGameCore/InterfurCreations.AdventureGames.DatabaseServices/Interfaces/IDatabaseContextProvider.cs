using InterfurCreations.AdventureGames.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.DatabaseServices.Interfaces
{
    public interface IDatabaseContextProvider
    {
        DatabaseContext GetContext();
        DatabaseContext GetNewContext();
        void Clear();
    }
}
