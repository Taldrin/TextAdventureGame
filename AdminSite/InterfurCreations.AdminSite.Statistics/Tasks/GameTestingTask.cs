using InterfurCreations.AdventureGames.GameTesting;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.BackgroundTasks.Tasks
{
    public class GameTestingTask : IBackgroundTask
    {
        private readonly IGameTestExecutor _testExecutor;
        private readonly IGameTestDataProvider _dataProvider;
        private readonly IGameRetrieverService _gameRetriever;

        public GameTestingTask(IGameTestExecutor gameTestExecutor, IGameTestDataProvider dataProvider,
            IGameRetrieverService gameRetriever)
        {
            _testExecutor = gameTestExecutor;
            _dataProvider = dataProvider;
            _gameRetriever = gameRetriever;
        }

        public void Run()
        {
            new GameTester(_testExecutor, _dataProvider, _gameRetriever).BeginTesting(null, 3, 0);
        }
    }
}
