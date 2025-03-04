using System;
using System.IO;
using System.Linq;
using System.Reflection;
using InterfurCreations.AdventureGames.BotMain.Tools;
using System.Collections.Generic;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services.Interfaces;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Services;

namespace InterfurCreations.AdventureGames.Graph.Store
{
    public class DrawStore : IGameStore
    {

        private IGoogleDriveService _service;

        public DrawStore(IGoogleDriveService driveService)
        {
            _service = driveService;
        }

        public List<string> CheckForOutOfDateGames(Dictionary<DrawGame, DateTime> timeRetrievedGames)
        {
            var fileList = _service.ListFiles();
            var outOfDateGames = timeRetrievedGames.Where(a => a.Value < fileList.Where(b => b.FileName.StartsWith(a.Key.GameName)).MaxBy(b => b.LastModified)?.LastModified);
            var newGames = fileList.Where(a => !timeRetrievedGames.Keys.Any(b => a.FileName.StartsWith(b.GameName)) && !a.FileName.Contains("_")).GroupBy(a => a.FileName).Select(a => a.First()).ToList();
            return outOfDateGames.ToList().Select(a => a.Key.GameName).ToList().Concat(newGames.Select(a => a.FileName)).ToList();
        }

        public (byte[] primary, List<byte[]> additional) GetGame(string game)
        {
            var foundGame = _service.ListFiles().Where(a => a.FileName.StartsWith(game)).GroupBy(a => a.FileName).Select(a => a.OrderByDescending(b => b.LastModified).FirstOrDefault()).ToList();
            var primary = foundGame.MinBy(a => a.FileName.Length);
            var primaryFileBytes = _service.DownloadFile(primary);
            foundGame.Remove(primary);

            var additionalFilesBytes = foundGame.Select(a => _service.DownloadFile(a)).ToList();

            return (primaryFileBytes, additionalFilesBytes);
        }
    }
}
