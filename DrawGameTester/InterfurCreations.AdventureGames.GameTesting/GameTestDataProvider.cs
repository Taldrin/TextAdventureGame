using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Database.GameTesting;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.GameTesting
{
    public class GameTestDataProvider : IGameTestDataProvider
    {
        private readonly IDatabaseContextProvider _database;

        public GameTestDataProvider(IDatabaseContextProvider database)
        {
            _database = database;
        }

        public GameTestDataStore ProvideDataForGameTesting(string gameName)
        {
           var dbContext = _database.GetContext();

            var optionsVisited = dbContext.GameTestingOptionVisited.Where(a => a.GameName == gameName).ToList();
            var statesVisited = dbContext.GameTestingStateVisited.Where(a => a.GameName == gameName).ToList();

            
            var dataStore = new GameTestDataStore();

            var optionsGrouped = optionsVisited.GroupBy(a => a.OptionId);
            dataStore.OptionVisits = optionsGrouped.ToDictionary(a => a.Key, v => v.Sum(b => b.TimesOccured));

            var stateVisitsGrouped = statesVisited.GroupBy(a => a.StateId);
            dataStore.StateVisits = stateVisitsGrouped.ToDictionary(a => a.Key, v => v.Sum(b => b.TimesOccured));
            return dataStore;
        }

        public GameTestingGameData ProvideData(string gameName)
        {
           var dbContext = _database.GetContext();

            var optionsVisited = dbContext.GameTestingOptionVisited.Where(a => a.GameName == gameName).ToList();
            var statesVisited = dbContext.GameTestingStateVisited.Where(a => a.GameName == gameName).ToList();
            var warnings = dbContext.GameTestingWarning.Where(a => a.GameName == gameName).Include(a => a.LatestGameSave).ThenInclude(a => a.SaveData).ToList();
            var errors = dbContext.GameTestingError.Where(a => a.GameName == gameName).Include(a => a.LatestGameSave).ThenInclude(a => a.SaveData).ToList();
            var endStates = dbContext.GameTestingEndStates.Where(a => a.GameName == gameName).Include(a => a.LatestGameSave).ThenInclude(a => a.SaveData).ToList();
            var grammar = dbContext.GameTestingGrammar.Where(a => a.GameName == gameName).Include(a => a.LatestGameSave).ThenInclude(a => a.SaveData).ToList();
            var misc = dbContext.GameTestingMiscData.Where(a => a.GameName == gameName).ToList();

            var optionsGrouped = optionsVisited.GroupBy(a => a.OptionId).Select(a => new GameTestingOptionVisited { GameName = a.First().GameName, OptionId = a.First().OptionId, TimesOccured = a.Sum(b => b.TimesOccured) }).ToList();
            var statesGrouped = statesVisited.GroupBy(a => a.StateId).Select(a => new GameTestingStateVisited { GameName = a.First().GameName, StateId = a.First().StateId, TimesOccured = a.Sum(b => b.TimesOccured) }).ToList();
            var variableNamesGrouped = misc.GroupBy(a => a.Value).Select(a => new GameTestingMiscData { GameName = a.First().GameName, Value = a.Key}).ToList();
            var warningsGrouped = warnings.DistinctBy(a => a.WarningMessage).ToList();
            var errorsGrouped = errors.DistinctBy(a => a.ErrorMessage).ToList();
            var grammarGrouped = grammar.DistinctBy(a => a.SpellingMessage).ToList();
            var endStatesGrouped = endStates.DistinctBy(a => a.EndState).ToList();

            return new GameTestingGameData
            {
                EndStates = endStatesGrouped,
                Errors = errorsGrouped,
                GrammarWarnings = grammarGrouped,
                MiscData = variableNamesGrouped,
                OptionsVisited = optionsGrouped,
                StatesVisited = statesGrouped,
                Warnings = warningsGrouped
            };
        }

        public void SaveDataForGame(GameTestDataStore dataStore, string gameName)
        {
            var dbContext = _database.GetContext();

            var optionsVisited = dbContext.GameTestingOptionVisited.Where(a => a.GameName == gameName).ToList();
            var statesVisited = dbContext.GameTestingStateVisited.Where(a => a.GameName == gameName).ToList();
            var warnings = dbContext.GameTestingWarning.Where(a => a.GameName == gameName).ToList();
            var errors = dbContext.GameTestingError.Where(a => a.GameName == gameName).ToList();
            var endStates = dbContext.GameTestingEndStates.Where(a => a.GameName == gameName).ToList();
            var grammar = dbContext.GameTestingGrammar.Where(a => a.GameName == gameName).ToList();
            var misc = dbContext.GameTestingMiscData.Where(a => a.GameName == gameName).ToList();

            // Options
            foreach(var item in dataStore.OptionVisits)
            {
                var existingItem =  optionsVisited.FirstOrDefault(a => a.OptionId == item.Key);
                if (existingItem == null)
                    dbContext.GameTestingOptionVisited.Add(new GameTestingOptionVisited
                    {
                        GameName = gameName,
                        OptionId = item.Key,
                        TimesOccured = item.Value,
                    });
                else
                    existingItem.TimesOccured = item.Value;
            }

            // States
            foreach(var item in dataStore.StateVisits)
            {
                var existingItem = statesVisited.FirstOrDefault(a => a.StateId == item.Key);
                if (existingItem == null)
                    dbContext.GameTestingStateVisited.Add(new GameTestingStateVisited
                    {
                        GameName = gameName,
                        StateId = item.Key,
                        TimesOccured = item.Value,
                    });
                else
                    existingItem.TimesOccured = item.Value;
            }

            // Warnings
            foreach(var item in dataStore.WarningMessages)
            {
                var existingItem = warnings.FirstOrDefault(a => a.WarningMessage == item.Value);
                if (existingItem == null)
                    dbContext.GameTestingWarning.Add(new GameTestingWarning
                    {
                        GameName = gameName,
                        WarningMessage = item.Value,
                        LatestGameSave = ToGameSave(item.GameSave),
                        TimesOccured = 1,
                    });
                else
                    existingItem.TimesOccured++;
            }

            // Errors
            foreach(var item in dataStore.ErrorMessages)
            {
                var existingItem = errors.FirstOrDefault(a => a.ErrorMessage == item.Value);
                if (existingItem == null)
                    dbContext.GameTestingError.Add(new GameTestingError
                    {
                        GameName = gameName,
                        ErrorMessage = item.Value,
                        LatestGameSave = ToGameSave(item.GameSave),
                        TimesOccured = 1,
                    });
                else
                    existingItem.TimesOccured++;
            }

            // End States
            foreach(var item in dataStore.PotentialEndStates)
            {
                var existingItem = endStates.FirstOrDefault(a => a.EndState == item.Value);
                if (existingItem == null)
                    dbContext.GameTestingEndStates.Add(new GameTestingEndState
                    {
                        GameName = gameName,
                        EndState = item.Value,
                        LatestGameSave = ToGameSave(item.GameSave),
                        TimesOccured = 1,
                    });
                else
                    existingItem.TimesOccured++;
            }

            // Grammar States
            foreach(var item in dataStore.GrammarMistakes)
            {
                var existingItem = grammar.FirstOrDefault(a => a.SpellingMessage == item.Value);
                if (existingItem == null)
                    dbContext.GameTestingGrammar.Add(new GameTestingGrammar
                    {
                        GameName = gameName,
                        SpellingMessage = item.Value,
                        LatestGameSave = ToGameSave(item.GameSave),
                        TimesOccured = 1,
                    });
                else
                    existingItem.TimesOccured++;
            }

            // Variable Names
            foreach(var item in dataStore.VariableNames)
            {
                var existingItem = misc.FirstOrDefault(a => a.Value == item.Value && a.Type == GameTestingMiscType.VariableName);
                if (existingItem == null)
                    dbContext.GameTestingMiscData.Add(new GameTestingMiscData
                    {
                        GameName = gameName,
                        Value = item.Value,
                        Type = GameTestingMiscType.VariableName
                    });
            }

            dbContext.SaveChanges();
        }

        public void DeleteDataForGame(string gameName)
        {
            var dbContext = _database.GetContext();
            var optionsVisited = dbContext.GameTestingOptionVisited.Where(a => a.GameName == gameName).ToList();
            var statesVisited = dbContext.GameTestingStateVisited.Where(a => a.GameName == gameName).ToList();
            var warnings = dbContext.GameTestingWarning.Where(a => a.GameName == gameName).ToList();
            var errors = dbContext.GameTestingError.Where(a => a.GameName == gameName).ToList();
            var endStates = dbContext.GameTestingEndStates.Where(a => a.GameName == gameName).ToList();
            var grammar = dbContext.GameTestingGrammar.Where(a => a.GameName == gameName).ToList();
            var misc = dbContext.GameTestingMiscData.Where(a => a.GameName == gameName).ToList();

            dbContext.GameTestingOptionVisited.RemoveRange(optionsVisited);
            dbContext.GameTestingStateVisited.RemoveRange(statesVisited);
            dbContext.GameTestingWarning.RemoveRange(warnings);
            dbContext.GameTestingError.RemoveRange(errors);
            dbContext.GameTestingEndStates.RemoveRange(endStates);
            dbContext.GameTestingGrammar.RemoveRange(grammar);
            dbContext.GameTestingMiscData.RemoveRange(misc);

            dbContext.SaveChanges();
        }

        private GameTestingGameSave ToGameSave(PlayerGameSave gameSave)
        {
            if (gameSave == null)
                return new GameTestingGameSave();
            return new GameTestingGameSave
            {
                GameName = gameSave.GameName,
                StateId = gameSave.StateId,
                SaveData = gameSave.GameSaveData.Select(a => new GameTestingGameSaveData
                {
                    DataName = a.Name,
                    DataValue = a.Value,
                }).ToList()
            };
        }
    }
}
