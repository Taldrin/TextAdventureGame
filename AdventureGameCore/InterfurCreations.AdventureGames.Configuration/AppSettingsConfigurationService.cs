using InterfurCreations.AdventureGames.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace InterfurCreations.AdventureGames.Configuration
{
    public class AppSettingsConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _config;

        public AppSettingsConfigurationService(IConfiguration config)
        {
            _config = config;
        }

        public string GetConfig(string configKey, bool dynamicConfig = false)
        {
            var value = GetConfigValue(configKey, dynamicConfig);
            if (value == null)
                throw new Exception("Error - config was null for key: " + configKey);
            return value;
        }

        public string GetConfigOrDefault(string configKey, string def, bool dynamicConfig = false)
        {
            var value = GetConfigValue(configKey, dynamicConfig);
            if (value == null)
                return def;
            return value;
        }

        public void SetConfig(string configKey, string value)
        {
            _config[$"Furventure:TextAdventures:{configKey}"] = value;
        }

        private string GetConfigValue(string config, bool dynamicConfig)
        {
            if(dynamicConfig)
            {
                var value = _config.GetSection($"Furventure:TextAdventures:{ConfigSetting.DynamicApplicationName}")[config];
                return value;
            } else
            {
                var value = _config.GetSection("Furventure:TextAdventures")[config];
                return value;
            }
        }
    }
}
