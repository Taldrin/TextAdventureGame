using InterfurCreations.AdminSite.Core.DataObjects;
using InterfurCreations.AdventureGames.Database.GameTesting;
using InterfurCreations.AdventureGames.GameTesting;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.Core
{
    public class GameTestingReportCompiler
    {
        private readonly IGameTestDataProvider _dataProvider;
        private readonly IGameRetrieverService _gameRetriever;

        public GameTestingReportCompiler(IGameTestDataProvider dataProvider, IGameRetrieverService gameRetriever)
        {
            _dataProvider = dataProvider;
            _gameRetriever = gameRetriever;
        }

        public GameTestingReportDataObject CompileReportForGame(string gameName)
        {
            var allData = _dataProvider.ProvideData(gameName);
            var game = _gameRetriever.ListGames().Find(a => a.GameName == gameName);

            var returnData = new GameTestingReportDataObject();

            returnData.Variables = allData.MiscData.Select(a => a.Value).OrderBy(a => a).ToList();

            returnData.EndStates = allData.EndStates.Select(a =>
            {
                var endState = game.FindStateById(a.EndState);
                var message = $"{new string(endState.StateText.Take(150).ToArray())}";
                return PrettifyData(a.LatestGameSave, message);
            }).ToList();

            returnData.Errors = allData.Errors.Select(a =>
            {
                var state = game.FindStateById(a.LatestGameSave.StateId);
                var message = $"{new string(state.StateText.Take(150).ToArray())}\nWith message: '{a.ErrorMessage}'";

                return PrettifyData(a.LatestGameSave, message);
            }).ToList();

            returnData.Grammar = allData.GrammarWarnings.Select(a =>
            {
                return PrettifyData(a.LatestGameSave, a.SpellingMessage);
            }).ToList();

            returnData.Warnings = allData.Warnings.Select(a =>
            {
                return PrettifyData(a.LatestGameSave, a.WarningMessage);
            }).ToList();

            var optionsNeverVisited = game.Stats.options.Where(a => !allData.OptionsVisited.Any(b => b.OptionId == a.Id)).Where(b => !string.IsNullOrWhiteSpace(b.StateText)).ToList();
            var statesNeverVisited = game.Stats.states.Where(a => !allData.StatesVisited.Any(b => b.StateId == a.Id)).ToList();

            returnData.StatesNeverVisited = statesNeverVisited.Select(a => $"'{a.StateText}' State ID: '{a.Id}'").ToList();
            returnData.OptionsNeverTaken = optionsNeverVisited.Select(a =>
            {
                return $"Option with text '{a.StateText}' pointing to state with text '{new string(a.ResultState.StateText.Take(150).ToArray())}' and ID '{a.ResultState.Id}'";
            }).ToList();

            return returnData;
        }

        public GameTestingReportItemWithSave PrettifyData(GameTestingGameSave gameSave, string message)
        {
            if (gameSave == null || gameSave.SaveData == null || gameSave.SaveData.Count == 0)
            {
                return new GameTestingReportItemWithSave
                {
                    Data = message,
                    SaveInfo = ""
                };
            }
            return new GameTestingReportItemWithSave
            {
                Data = message,
                SaveInfo = string.Join(" | ", gameSave?.SaveData.Select(a => $"{a.DataName} - {a.DataValue}"))
            };
        }

        public void DeleteAllDataForGame(string gameName)
        {
            _dataProvider.DeleteDataForGame(gameName);
        }
    }
}
