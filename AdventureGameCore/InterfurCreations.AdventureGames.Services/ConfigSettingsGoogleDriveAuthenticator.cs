using InterfurCreations.AdventureGames.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.IO;
using InterfurCreations.AdventureGames.Logging;

namespace InterfurCreations.AdventureGames.Services
{
    public class ConfigSettingsGoogleDriveAuthenticator : IGoogleDriveAuthenticator
    {
        protected virtual string[] GetScopes()
        {
            return new string[] { DriveService.Scope.DriveReadonly };
        }

        public DriveService AuthenticateService(IConfigurationService configService)
        {

            Log.LogMessage("Looking for google drive credentials in configuration...");
            var credentialJson = configService.GetConfig("GoogleDrive");
            var credential = GoogleCredential.FromJson(credentialJson).CreateScoped(GetScopes());

            // Create Drive API service.
            var gDriveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Adventure Game Draw.IO Parser",
            });
            return gDriveService;
        }
    }
}
