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
        private readonly string DirectoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"DrawFiles\"); //+ "DrawIoTest" + ".xml");
        private Dictionary<DrawGame, DateTime> TimeRetrievedGame = new Dictionary<DrawGame, DateTime>();

        private DateTime LastChecked;
        private DrawParser _parser;
        private IGoogleDriveService _service;

        private IConfigurationService _configService;
        private IReporter _reporter;

        public DrawStore(IConfigurationService configService, IReporter reporter, IGoogleDriveService driveService)
        {
            _reporter = reporter;
            _parser = new DrawParser();
            _configService = configService;
            _service = driveService;
        }

        public void Initialise()
        {
            ScanForNewFiles();
        }

        public List<DrawGame> ListGames()
        {
            var minutesBetweenChecks = int.Parse(_configService.GetConfigOrDefault("MaxMinutesBetweenGameCheck", "10", true));
            if (LastChecked.Add(TimeSpan.FromMinutes(minutesBetweenChecks)) < DateTime.UtcNow)
                CheckForOutOfDate();
            return TimeRetrievedGame.Keys.ToList();
        }

        public List<DrawGame> ListGames(TimeSpan timeBetweenCheck)
        {
            if(LastChecked.Add(timeBetweenCheck) < DateTime.UtcNow)
                CheckForOutOfDate();
            return TimeRetrievedGame.Keys.ToList();
        }

        private void CheckForOutOfDate()
        {
            LastChecked = DateTime.UtcNow;
            ScanForNewFiles();
            var fileList = _service.ListFiles();
            var outOfDateGames = TimeRetrievedGame.Where(a => a.Value < fileList.FirstOrDefault(b => b.FileName == a.Key.GameName).LastModified);
            outOfDateGames.ToList().ForEach(a => UpdateGame(a.Key));
        }

        private void UpdateGame(DrawGame game)
        {
            var foundGame = _service.ListFiles().Where(a => a.FileName == game.GameName).OrderByDescending(a => a.LastModified).FirstOrDefault();

            var existingGame = TimeRetrievedGame.FirstOrDefault(a => a.Key.GameName == game.GameName).Key;
            TimeRetrievedGame.Remove(existingGame);

            AddGame(foundGame);
        }

        private void ScanForNewFiles()
        {
            var allFiles = _service.ListFiles();
            allFiles.ToList().ForEach(a =>
            {
                var name = a.FileName;
                if(!TimeRetrievedGame.Any(b => b.Key.GameName == name))
                {
                    AddGame(a);
                }
            });
        }

        private void AddGame(GoogleFile file)
        {
            var xmlBytes = _service.DownloadFile(file);
            DateTime lastModified;
            if (file.LastModified.HasValue)
                lastModified = file.LastModified.Value;
            else
                lastModified = DateTime.UtcNow;
            try
            {
                var parsedGame = _parser.ParseGameFromBytes(xmlBytes);
                var newGame = new DrawGame { GameName = file.FileName, StartState = parsedGame.game, Metadata = parsedGame.metadata, GameFunctions = parsedGame.functions};
                var stats = OptionsCountTool.Run(newGame);
                newGame.Stats = stats;
                if (_reporter != null)
                    _reporter.ReportMessage($"Parsed a new game with name: {newGame.GameName}. It has {stats.optionsCount} decisions/options, {stats.states.Count} unique states, and {stats.wordCount} words!");
                TimeRetrievedGame.Add(newGame, lastModified);
            }
            catch (Exception e) { Log.LogMessage("Critical error parsing game with name: " + file.FileName + " with error message: " + e.Message, LogType.Error, e.StackTrace); return; }
        }
    }
}
