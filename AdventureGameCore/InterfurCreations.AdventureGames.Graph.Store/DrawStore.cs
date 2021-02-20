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
            var outOfDateGames = timeRetrievedGames.Where(a => a.Value < fileList.FirstOrDefault(b => b.FileName == a.Key.GameName).LastModified);
            var newGames = fileList.Where(a => !timeRetrievedGames.Keys.Any(b => b.GameName == a.FileName)).GroupBy(a => a.FileName).Select(a => a.First()).ToList();
            return outOfDateGames.ToList().Select(a => a.Key.GameName).ToList().Concat(newGames.Select(a => a.FileName)).ToList();
        }

        public byte[] GetGame(string game)
        {
            var foundGame = _service.ListFiles().Where(a => a.FileName == game).OrderByDescending(a => a.LastModified).FirstOrDefault();
            var gameBytes = _service.DownloadFile(foundGame);
            return gameBytes;
        }
    }
}
