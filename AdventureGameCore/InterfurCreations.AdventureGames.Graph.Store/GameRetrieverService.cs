using InterfurCreations.AdventureGames.BotMain.Tools;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfurCreations.AdventureGames.Graph.Store
{
    public class GameRetrieverService : IGameRetrieverService
    {
        private readonly IGameStore _gameStore;
        private readonly IReporter _reporter;
        private readonly IConfigurationService _configService;
        private DrawParser _parser;

        private Dictionary<DrawGame, DateTime> TimeRetrievedGame = new Dictionary<DrawGame, DateTime>();
        private DateTime LastChecked;

        private object lockObj = new object();

        private readonly List<string> _filteredGameNames = new List<string>()
        {
            "liveconfiguration"
        };

        public GameRetrieverService(IGameStore gameStore, IReporter reporter, IConfigurationService configService)
        {
            _gameStore = gameStore;
            _reporter = reporter;
            _configService = configService;
            _parser = new DrawParser();
        }

        public List<DrawGame> ListGames(bool includeFilteredGames = false)
        {
            lock (lockObj)
            {
                var minutesBetweenChecks = int.Parse(_configService.GetConfigOrDefault("MaxMinutesBetweenGameCheck", "10", true));
                if (LastChecked.Add(TimeSpan.FromMinutes(minutesBetweenChecks)) < DateTime.UtcNow)
                    CheckForOutOfDate();
                if(!includeFilteredGames)
                    return TimeRetrievedGame.Keys.Where(a => !_filteredGameNames.Contains(a.GameName.ToLowerInvariant())).ToList();
                else
                    return TimeRetrievedGame.Keys.ToList();
            }
        }

        public List<DrawGame> ListGames(TimeSpan timeBetweenCheck, bool includeFilteredGames = false)
        {
            lock (lockObj)
            {
                if (LastChecked.Add(timeBetweenCheck) < DateTime.UtcNow)
                    CheckForOutOfDate();
                if (!includeFilteredGames)
                    return TimeRetrievedGame.Keys.Where(a => !_filteredGameNames.Contains(a.GameName.ToLowerInvariant())).ToList();
                else
                    return TimeRetrievedGame.Keys.ToList();
            }
        }

        private void CheckForOutOfDate()
        {

            LastChecked = DateTime.UtcNow;
            var oldGames = _gameStore.CheckForOutOfDateGames(TimeRetrievedGame);

            foreach (var gameToUpdate in oldGames)
            {
                var existingGame = TimeRetrievedGame.Keys.FirstOrDefault(a => a.GameName == gameToUpdate);
                if (existingGame == null)
                {
                    var newGame = ParseGame(_gameStore.GetGame(gameToUpdate), gameToUpdate);
                    if (newGame != null)
                        AddNewGame(newGame);
                }
                else
                {
                    var updatedGame = ParseGame(_gameStore.GetGame(gameToUpdate), gameToUpdate);
                    if (updatedGame != null)
                        AddExistingGame(updatedGame, existingGame);
                }
            }
        }

        private DrawGame ParseGame(byte[] file, string gameName)
        {
            try
            {
                var parsedGame = _parser.ParseGameFromBytes(file);
                var newGame = new DrawGame { GameName = gameName, StartState = parsedGame.game, Metadata = parsedGame.metadata, GameFunctions = parsedGame.functions };
                var stats = OptionsCountTool.Run(newGame);
                newGame.Stats = stats;
                return newGame;
            }
            catch (Exception e) { Log.LogMessage("Critical error parsing game with name: " + gameName + " with error message: " + e.Message, LogType.Error, e.StackTrace); return null; }
        }

        private void AddNewGame(DrawGame newGame)
        {
            if (_reporter != null)
                _reporter.ReportMessage($"Parsed a new game with name: {newGame.GameName}. It has {newGame.Stats.optionsCount} decisions/options, " +
                    $"{newGame.Stats.states.Count} unique states, and {newGame.Stats.wordCount} words!");
            TimeRetrievedGame.Add(newGame, DateTime.UtcNow);
        }

        private void AddExistingGame(DrawGame newGame, DrawGame existingGame)
        {
            if (_reporter != null)
                _reporter.ReportMessage($"Updated game {newGame.GameName}. It has {newGame.Stats.optionsCount} (+{newGame.Stats.optionsCount - existingGame.Stats.optionsCount}) decisions/options, " +
                    $"{newGame.Stats.states.Count} (+{newGame.Stats.states.Count - existingGame.Stats.states.Count}) unique states, and {newGame.Stats.wordCount} (+{newGame.Stats.wordCount - existingGame.Stats.wordCount}) words!");
            TimeRetrievedGame.Remove(existingGame);
            TimeRetrievedGame.Add(newGame, DateTime.UtcNow);
        }
    }
}
