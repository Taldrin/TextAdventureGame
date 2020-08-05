using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.DatabaseServices
{
    public class DatabaseDataStore : IDataStore
    {
        private readonly DatabaseContext _databaseContext;

        public DatabaseDataStore(IDatabaseContextProvider contextProvider)
        {
            _databaseContext = contextProvider.GetContext();
        }

        public void SaveChanges()
        {
            _databaseContext.SaveChanges();
        }
    }
}
