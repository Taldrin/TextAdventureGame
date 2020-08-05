using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Configuration
{
    public interface IConfigurationService
    {
        string GetConfig(string configKey);
        string GetConfigOrDefault(string configKey, string def);
    }
}
