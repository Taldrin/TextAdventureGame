﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        public ConfigurationService()
        {
        }

        public string GetConfig(string configKey, bool dynamicConfig = false)
        {
            var value = ConfigurationManager.AppSettings[configKey];
            if (value == null)
                throw new Exception("Error - config was null for key: " + configKey);
            return value;
        }

        public string GetConfigOrDefault(string configKey, string def, bool dynamicConfig = false)
        {
            var value = ConfigurationManager.AppSettings.AllKeys.SingleOrDefault(a => a == configKey);
            if (value == null)
                return def;
            return ConfigurationManager.AppSettings[value];
        }

        public void SetConfig(string configKey, string value)
        {
            ConfigurationManager.AppSettings[configKey] = value;
        }
    }
}
