using Furventure.AdventureGames.Offline.Database;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furventure.AdventureGames.DatabaseServices.Offline
{
    public class OfflineDatabaseContextProvider : IDatabaseContextProvider
    {
        private OfflineDatabaseContext cachedContext;

        public OfflineDatabaseContextProvider()
        {
        }

        public DatabaseContext GetContext()
        {
            if (cachedContext == null)
                cachedContext = new OfflineDatabaseContext();
            return cachedContext;
        }

        public DatabaseContext GetNewContext()
        {
            cachedContext = new OfflineDatabaseContext();
            return cachedContext;
        }

        public void Clear()
        {
            if (cachedContext != null)
            {
                cachedContext.Dispose();
                cachedContext = null;
            }
        }
    }
}
