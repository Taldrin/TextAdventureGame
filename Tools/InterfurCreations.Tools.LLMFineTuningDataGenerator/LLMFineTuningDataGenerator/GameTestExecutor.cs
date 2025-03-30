using InterfurCreations.AdventureGames.GameTesting;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMFineTuningDataGenerator
{
    public class GameTestExecutor
    {
        private readonly IGameRetrieverService _gameStore;
        private readonly DrawGameTestExecutor _executor;
        private readonly IReporter _reporter;

        public GameTestExecutor(IGameRetrieverService gameStore, DrawGameTestExecutor drawGameTestExecutor, IReporter reporter)
        {
            _gameStore = gameStore;
            _executor = drawGameTestExecutor;
            _reporter = reporter;
        }

        public List<List<GameTestMessageResult>> Execute(string gameName, int timesToRun = 5)
        {
            var game = _gameStore.ListGames().FirstOrDefault(a => a.GameName == gameName);
            List<List<GameTestMessageResult>> InstanceResults = new List<List<GameTestMessageResult>>();
            for (int i = 0; i < timesToRun; i++)
            {
                _executor.RunTestAsync(game, DateTime.MaxValue, 0, new GameTestDataStore(), runOnce: true).Wait();
                InstanceResults.Add(_executor.gameTestMessageResults);
            }

            return InstanceResults;
        }
    }
}
