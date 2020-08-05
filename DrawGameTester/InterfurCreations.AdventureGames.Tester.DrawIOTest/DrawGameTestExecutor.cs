using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Tester.DrawIOTest
{
    public class DrawGameTestExecutor
    {
        private IGameProcessor _gameProcessor;
        private HashSet<string> warnings = new HashSet<string>();
        private HashSet<string> errors = new HashSet<string>();

        private GameTestData data;

        public DrawGameTestExecutor(IGameProcessor gameProcessor)
        {
            _gameProcessor = gameProcessor;
        }

        public (List<string> errors, List<string> warnings, int totalActionsDone) RunTest(DrawGame game, DateTime runUntil)
        {
            data = new GameTestData();
            Random rand = new Random();
            var gameState = new PlayerGameSave();
            gameState.GameName = game.GameName;
            gameState.StateId = game.startState.Id;

            int totalActionsDone = 0;

            while (DateTime.Now <= runUntil)
            {
                try
                {
                    data.StateVisited(gameState.StateId);

                    var options = _gameProcessor.GetCurrentOptionsFullDrawData(gameState, game);

                    if (options == null || options.Count == 0)
                    {
                        ReportWarning("No options found - Data:" + PrettifyData(gameState), gameState, game);
                        gameState = new PlayerGameSave();
                        gameState.GameName = game.GameName;
                        gameState.StateId = game.startState.Id;
                        continue;
                    }
                    options.ForEach(a =>
                    {
                        if (options.Count(b => b.option.ToLower() == a.option.ToLower()) > 1)
                            ReportError("Found a state with two or more options with the same text: " + a, gameState, game);
                    });

                    (StateOption option, string message, int timesChosen) minValue = (options[0].optionData, options[0].option, data.GetTimesChosen(options[0].optionData.Id));
                    foreach(var opt in options)
                    {
                        var optionTimesChosen = data.GetTimesChosen(opt.optionData.Id);
                        if (optionTimesChosen < minValue.timesChosen)
                            minValue = (opt.optionData, opt.option, optionTimesChosen);
                        else if(optionTimesChosen == minValue.timesChosen)
                        {
                            if(rand.Next(1) == 0)
                                minValue = (opt.optionData, opt.option, optionTimesChosen);
                        }
                            
                    }
                    var optionToExecute = options[rand.Next(options.Count)];

                    var execResult = _gameProcessor.ProcessMessage(optionToExecute.option, gameState, game);
                    execResult.StatesVisited.ForEach(a => data.StateVisited(a));
                    data.OptionChosen(optionToExecute.optionData.Id);
                    totalActionsDone++;
                }
                catch (Exception e)
                {
                    ReportError("ERROR: " + e.Message + "\nSTACK: " + e.StackTrace + "\nDATA: " + PrettifyData(gameState), gameState, game);
                    gameState = new PlayerGameSave();
                    gameState.GameName = game.GameName;
                    gameState.StateId = game.startState.Id;
                    continue;
                }
            }
            var statesNeverVisited = game.Stats.states.Select(a => a.Id).Except(data.GetAllStatesVisited());
            statesNeverVisited.ToList().ForEach(a =>
            {
                var state = game.FindStateById(a);
                errors.Add($"State never visited with text: '{(!string.IsNullOrWhiteSpace(state.StateText) && state.StateText.Length >= 300 ? state.StateText.Substring(0, 300) : state.StateText)}'");
            });

            return (errors.ToList(), warnings.ToList(), totalActionsDone);
        }

        private void ReportWarning(string text, PlayerGameSave gameSave, DrawGame game)
        {
            var state = game.FindStateById(gameSave.StateId);
            var mText = $"Warning '{text}' in state with text: '{(!string.IsNullOrWhiteSpace(state.StateText) && state.StateText.Length >= 300 ? state.StateText.Substring(0, 300) : state.StateText)}'";
            warnings.Add(mText);
        }

        private void ReportError(string text, PlayerGameSave gameSave, DrawGame game)
        {
            var state = game.FindStateById(gameSave.StateId);
            var mText = $"Error '{text}' \nIn state with text: '{(!string.IsNullOrWhiteSpace(state.StateText) && state.StateText.Length >= 300 ? state.StateText.Substring(0, 300) : state.StateText)}'";
            errors.Add(mText);
        }

        private string PrettifyData(PlayerGameSave gameState)
        {
            return "";//string.Join(" ", gameState.GameSaveData.Select(a => $"[Name: '{a.Name}' Value: '{a.Value}']"));
        }
    }
}
