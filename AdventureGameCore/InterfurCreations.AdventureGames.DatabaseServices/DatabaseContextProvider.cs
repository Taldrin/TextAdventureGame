using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.DatabaseServices
{
    public class DatabaseContextProvider : IDatabaseContextProvider
    {
        private DatabaseContext cachedContext;
        private readonly IConfigurationService _configService;

        public DatabaseContextProvider(IConfigurationService configService)
        {
            _configService = configService;
        }

        public DatabaseContext GetContext()
        {
            if (cachedContext == null)
                cachedContext = new DatabaseContext(_configService);
            return cachedContext;
        }

        public DatabaseContext GetNewContext()
        {
            cachedContext = new DatabaseContext(_configService);
            return cachedContext;
        }

        public void Clear()
        {
            if (cachedContext != null)
                cachedContext.Dispose();
        }
    }
}
