using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.GameTesting;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.GameTesting
{
    public class DrawGameTestExecutor : IGameTestExecutor
    {
        private IGameProcessor _gameProcessor;
        private ISpellChecker _spellChecker;

        private HashSet<string> allText = new HashSet<string>();

        private Dictionary<string, List<StateVisit>> stateVisits = new Dictionary<string, List<StateVisit>>();

        private GameTestDataStore data;

        private DrawState startingState;
        private PlayerGameSave gameState;
        private DrawGame game;
        private Random rand;

        private int totalActionsDone = 0;
        private int actionsDoneThisRun = 0;
        private int actionsPerRun;
        private int previousDataLength = 0;

        public DrawGameTestExecutor(IGameProcessor gameProcessor, ISpellChecker spellChecker)
        {
            _gameProcessor = gameProcessor;
            _spellChecker = spellChecker;
        }

        private void ResetGame()
        {
            gameState = new PlayerGameSave();
            gameState.GameName = game.GameName;
            gameState.StateId = startingState.Id;
            actionsDoneThisRun = 0;
            previousDataLength = 0;
        }

        private (string option, StateOption optionData)? PickOption(DrawState state)
        {

            var options = _gameProcessor.GetCurrentOptionsFullDrawData(gameState, game).Where(a => !game.Metadata.PermanentButtons.Any(b => b.ButtonText == a.option)).ToList();

            if (options == null || options.Count == 0)
                return null;

            options.ForEach(a =>
            {
                if (options.Count(b => b.option.ToLower() == a.option.ToLower()) > 1)
                    data.WarningMessage($"Found a state with two or more options with the same text '{a}' at state with text '{new string(state.StateText.Take(150).ToArray())}'", gameState.Clone());
            });

            // Select the option with the fewest times chosen
            //(StateOption option, string message, int timesChosen) minValue = (options[0].optionData, options[0].option, data.GetTimesChosen(options[0].optionData.Id));
            //foreach (var opt in options)
            //{
            //    var optionTimesChosen = data.GetTimesChosen(opt.optionData.Id);
            //    if (optionTimesChosen < minValue.timesChosen)
            //        minValue = (opt.optionData, opt.option, optionTimesChosen);
            //    else if (optionTimesChosen == minValue.timesChosen)
            //    {
            //        if (rand.Next(1) == 0)
            //            minValue = (opt.optionData, opt.option, optionTimesChosen);
            //    }
            //}

            // Select an option, preferring those that have been visited fewer times
            var chosen = WeightedRandom(options.Select(a => (a, data.GetTimesChosen(a.optionData.Id))).ToList());
            return chosen;

            // Full random
            //return options[rand.Next(options.Count)];
        }

        private T WeightedRandom<T>(List<(T data, int weight)> items) 
        {
            if (items == null || items.Count == 0)
                return default;
            if (items.Count == 1)
                return items[0].data;

            int totalWeight = items.Sum(a => a.weight);
            var choice = rand.Next(totalWeight);

            int sum = 0;
            for(int i = 0; i < items.Count; i++)
            {
                sum = sum + Math.Abs(items[i].weight - totalWeight);
                if (choice <= sum)
                    return items[i].data;
            }
            throw new ArgumentException("Error with weighted random. Found no choice");
        }

        public async Task RunTestAsync(DrawGame drawGame, DateTime runUntil, int actionsPerRunOption, GameTestDataStore dataStore, string startingStateId = null)
        {
            rand = new Random();

            actionsPerRun = actionsPerRunOption;
            game = drawGame;
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

            data = dataStore;
            if (dataStore == null)
                data = new GameTestDataStore();

            ResetGame();


            while (DateTime.Now <= runUntil)
            {
                try
                {
                    var state = game.FindStateById(gameState.StateId);

                    data.StateVisited(gameState.StateId);

                    if (actionsDoneThisRun > actionsPerRun)
                        break;

                    var optionToExecute = PickOption(state);

                    if (optionToExecute == null)
                    {
                        ResetGame();
                        data.PotentialEndStateFound(state.Id, gameState.Clone());
                        continue;
                    }

                    var execResult = _gameProcessor.ProcessMessage(optionToExecute.Value.option, gameState, game, new Player());
                    
                    execResult.StatesVisited.ForEach(a => data.StateVisited(a));
                    data.OptionChosen(optionToExecute.Value.optionData.Id);

                    execResult.MessagesToShow.ForEach(a => allText.Add(a.Message));

                    if(gameState.GameSaveData.Count > previousDataLength)
                    {
                        data.CheckForNewVariables(gameState);
                    }

                    totalActionsDone++;
                    actionsDoneThisRun++;
                    previousDataLength = gameState.GameSaveData.Count;
                }
                catch (Exception e)
                {
                    data.ErrorMessageEncountered(e.Message + "\n" + e.StackTrace, gameState.Clone());
                    ResetGame();
                    continue;
                }
            }

            // Azure grammar checking takes too long and costs too much,
            // MSWord spell checking does not work on Linux,
            // and LanguageTool spell checking is just a bit too memory-intensive for our server
            // :( 

            //var spellingResults = new ConcurrentBag<string>();
            //Parallel.ForEach(allText, new ParallelOptions { MaxDegreeOfParallelism = 12 }, item =>
            //{
            //    var spelling =  _spellChecker.CheckSpellingAsync(item).Result;
            //    if (spelling != null && spelling.suggestions.Count > 0)
            //    {
            //        var suggestions = string.Join(" | ", spelling.suggestions.Select(a => $"'{a.original}' -> '{a.message}' suggested change: '{a.suggestion}'\n"));
            //        var toReturn = $"State with text: {new string(item.Take(150).ToArray())}\n{suggestions}";
            //        spellingResults.Add(toReturn);
            //    }
            //});
            //foreach (var item in spellingResults)
            //    data.GrammarMistakeFound(item, null);
        }

        private string PrettifyData(PlayerGameSave gameState)
        {
            return "";//string.Join(" ", gameState.GameSaveData.Select(a => $"[Name: '{a.Name}' Value: '{a.Value}']"));
        }
    }
}
