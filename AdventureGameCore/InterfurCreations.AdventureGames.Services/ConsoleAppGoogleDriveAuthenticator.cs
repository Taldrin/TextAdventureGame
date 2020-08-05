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
    public class ConsoleAppGoogleDriveAuthenticator : IGoogleDriveAuthenticator
    {
        private static string[] Scopes = { DriveService.Scope.DriveReadonly };

        public DriveService AuthenticateService(IConfigurationService configService)
        {
            GoogleCredential credential;

            var filePath = AppDomain.CurrentDomain.BaseDirectory + "credentials.json";

            Log.LogMessage("Looking for credential file at: " + filePath);

            using (var stream =
                new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleCredential.FromFile(filePath).CreateScoped(Scopes);
            }

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
