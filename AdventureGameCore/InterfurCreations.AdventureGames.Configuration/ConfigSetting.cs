using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.Configuration
{
    public class ConfigSetting
    {
        // Used to specify different configuration values, for values which may have duplicates
        // i.e - we host 3 different telegram bots, live, alpha and dev. They need 3 seperate API keys
        public static string DynamicApplicationName;
    }
}
