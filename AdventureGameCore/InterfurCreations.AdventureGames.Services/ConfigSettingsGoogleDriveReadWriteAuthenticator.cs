using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services
{
    public class ConfigSettingsGoogleDriveReadWriteAuthenticator : ConfigSettingsGoogleDriveAuthenticator
    {
        protected override string[] GetScopes()
        {
            return new string[] 
            { 
                DriveService.Scope.Drive 
            };
        }
    }
}
