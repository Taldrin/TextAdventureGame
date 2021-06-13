using InterfurCreations.AdventureGames.GameTesting;
using InterfurCreations.AdventureGames.Graph.Store;

namespace InterfurCreations.AdminSite.BackgroundTasks.Tasks
{
    public class CustomGameTestTask
    {
        private readonly IGameTestExecutor _testExecutor;
        private readonly IGameTestDataProvider _dataProvider;
        private readonly IGameRetrieverService _gameRetriever;

        public CustomGameTestTask(IGameTestExecutor gameTestExecutor, IGameTestDataProvider dataProvider,
            IGameRetrieverService gameRetriever)
        {
            _testExecutor = gameTestExecutor;
            _dataProvider = dataProvider;
            _gameRetriever = gameRetriever;
        }

        public void Run(string gameName, int minutes, int maxActions, string startState)
        {
            new GameTester(_testExecutor, _dataProvider, _gameRetriever).BeginTesting(gameName, minutes, maxActions, startState);
        }
    }
}
