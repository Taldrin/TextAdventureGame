using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;


namespace InterfurCreations.AdventureGames.Services.Interfaces
{
    public interface IGoogleDriveService
    {
        List<GoogleFile> ListFiles();

        byte[] DownloadFile(GoogleFile file);
        void UploadFile(string name, string folderName, Stream uploadStream);
    }
}
