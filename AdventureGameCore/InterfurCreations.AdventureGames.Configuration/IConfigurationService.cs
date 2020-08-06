using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Configuration
{
    public interface IConfigurationService
    {
        string GetConfig(string configKey, bool dynamicConfig = false);
        string GetConfigOrDefault(string configKey, string def, bool dynamicConfig = false);
        void SetConfig(string configKey, string value);
    }
}
