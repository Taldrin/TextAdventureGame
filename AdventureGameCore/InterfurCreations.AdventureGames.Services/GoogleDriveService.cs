﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InterfurCreations.AdventureGames.Configuration;
using Google.Apis.Drive.v3;
using InterfurCreations.AdventureGames.Services.Interfaces;

namespace InterfurCreations.AdventureGames.Services
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private DriveService gDriveService;
        private IConfigurationService _configService;
        private IGoogleDriveAuthenticator _authenticator;

        public GoogleDriveService(IConfigurationService configService, IGoogleDriveAuthenticator googleDriveAuthenticator)
        {
            _configService = configService;
            _authenticator = googleDriveAuthenticator;
        }

        public List<GoogleFile> ListFiles()
        {
            if (gDriveService == null)
                gDriveService = _authenticator.AuthenticateService(_configService);
            string folderName = _configService.GetConfig("GoogleFolder", true);
            FilesResource.ListRequest listRequest = gDriveService.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, owners, parents, modifiedTime)";
            listRequest.PageSize = 1000;

            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            var folder = files.FirstOrDefault(a => a.Name == folderName);
            if (folder == null)
                throw new Exception("There was a critical error trying to find game files online - Online game folder not found");
            var folderId = folder.Id;
            var fileList = files.Where(a => a.Parents != null && a.Parents.Contains(folderId)).ToList();

            return fileList.Select(a => new GoogleFile { FileName = Path.GetFileNameWithoutExtension(a.Name), Id = a.Id, LastModified = a.ModifiedTime }).ToList();
        }

        public byte[] DownloadFile(GoogleFile file)
        {
            var request = gDriveService.Files.Get(file.Id);
            var ms = new MemoryStream();
            request.Download(ms);
            return ms.ToArray();
        }

    }
}
