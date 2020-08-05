using InterfurCreations.AdventureGames.Configuration;
using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services.Interfaces
{
    public interface IGoogleDriveAuthenticator
    {
        DriveService AuthenticateService(IConfigurationService configService);
    }
}
