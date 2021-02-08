using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.GameTesting;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Tester.DrawIOTest
{
    public class DrawGameTestExecutor : IGameTestExecutor
    {
        private IGameProcessor _gameProcessor;
        private ISpellChecker _spellChecker;
        private HashSet<string> warnings = new HashSet<string>();
        private HashSet<string> errors = new HashSet<string>();
        private HashSet<string> spelling = new HashSet<string>();

        private HashSet<string> allText = new HashSet<string>();

        private Dictionary<string, List<StateVisit>> stateVisits = new Dictionary<string, List<StateVisit>>();

        private GameTestData data;

        private DrawState startingState;

        public DrawGameTestExecutor(IGameProcessor gameProcessor, ISpellChecker spellChecker)
        {
            _gameProcessor = gameProcessor;
            _spellChecker = spellChecker;
        }

        public async Task<(List<string> errors, List<string> warnings, List<string> allText, int totalActionsDone)> RunTestAsync(DrawGame game, DateTime runUntil, int actionsPerRun, string startingStateId = null)
        {
            if (actionsPerRun == 0)
                actionsPerRun = int.MaxValue;

            if(!string.IsNullOrEmpty(startingStateId))
            {
                startingState = game.FindStateById(startingStateId);
                if (startingState == null)
                    throw new ArgumentException($"Could not find state '{startingStateId}' in game '{game.GameName}");
            } else
            {
                startingState = game.StartState;
            }

            data = new GameTestData();
            Random rand = new Random();
            var gameState = new PlayerGameSave();
            gameState.GameName = game.GameName;

            gameState.StateId = startingState.Id;

            int totalActionsDone = 0;
            int actionsDoneThisRun = 0;

            while (DateTime.Now <= runUntil)
            {
                try
                {
                    data.StateVisited(gameState.StateId);

                    var state = game.FindStateById(gameState.StateId);

                    var options = _gameProcessor.GetCurrentOptionsFullDrawData(gameState, game);

                    if (options == null || options.Count == 0 || actionsDoneThisRun > actionsPerRun)
                    {
                        gameState = new PlayerGameSave();
                        gameState.GameName = game.GameName;
                        gameState.StateId = startingState.Id;
                        actionsDoneThisRun = 0;
                        ReportWarning("Potential end state found with text: " + state.StateText, gameState, game);
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

                    var execResult = _gameProcessor.ProcessMessage(optionToExecute.option, gameState, game, new Player());
                    execResult.StatesVisited.ForEach(a => data.StateVisited(a));
                    data.OptionChosen(optionToExecute.optionData.Id);

                    execResult.MessagesToShow.ForEach(a => allText.Add(a.Message));

                    for (int i = 0; i > execResult.StatesVisited.Count; i++)
                    {
                        var stateId = execResult.StatesVisited[i];
                        var stateVisit = new StateVisit
                        {
                            SaveData = CloneData(gameState.GameSaveData),
                            StateId = stateId,
                            Text = execResult.MessagesToShow[i].Message
                        };
                        if (i == execResult.StatesVisited.Count)
                            stateVisit.Options = execResult.OptionsToShow;
                        if (stateVisits.TryGetValue(stateId, out var existingVisits))
                        {
                            if (!existingVisits.Any(a => a.Text == stateVisit.Text))
                                existingVisits.Add(stateVisit);
                        }
                        else
                        {
                            stateVisits.Add(stateId, new List<StateVisit> { stateVisit });
                        }
                    }

                    totalActionsDone++;
                    actionsDoneThisRun++;
                }
                catch (Exception e)
                {
                    ReportError("ERROR: " + e.Message + "\nSTACK: " + e.StackTrace + "\nDATA: " + PrettifyData(gameState), gameState, game);
                    gameState = new PlayerGameSave();
                    gameState.GameName = game.GameName;
                    gameState.StateId = startingState.Id;
                    continue;
                }
            }
            var statesNeverVisited = game.Stats.states.Select(a => a.Id).Except(data.GetAllStatesVisited());
            statesNeverVisited.ToList().ForEach(a =>
            {
                var state = game.FindStateById(a);
                errors.Add($"State never visited with text: '{(!string.IsNullOrWhiteSpace(state.StateText) && state.StateText.Length >= 300 ? state.StateText.Substring(0, 300) : state.StateText)}'");
            });

            return (errors.ToList(), warnings.ToList(), allText.ToList(), totalActionsDone);
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

        private List<PlayerGameSaveData> CloneData(List<PlayerGameSaveData> data)
        {
            return data.Select(a =>
            {
                return new PlayerGameSaveData
                {
                    Id = a.Id,
                    Name = a.Name,
                    PlayerGameSaveSaveId = a.PlayerGameSaveSaveId,
                    Value = a.Value
                };
            }).ToList();
        }
    }
}
