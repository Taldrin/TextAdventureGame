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

        public string GetConfig(string configKey)
        {
            var value = GetConfigValue(configKey);
            if (value == null)
                throw new Exception("Error - config was null for key: " + configKey);
            return value;
        }

        public string GetConfigOrDefault(string configKey, string def)
        {
            var value = GetConfigValue(configKey);
            if (value == null)
                return def;
            return value;
        }

        private string GetConfigValue(string config)
        {
            var value = _config.GetSection("InterfurSettings").GetSection(config).Value;
            if (value == null)
            {
                value = Environment.GetEnvironmentVariable(config);
            }
            return value;
        }
    }
}
