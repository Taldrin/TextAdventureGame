using InterfurCreations.AdventureGames.GameTesting;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdminSite.BackgroundTasks
{
    public class GameTester
    {
        private readonly IGameTestExecutor _testExecutor;
        private readonly IGameTestDataProvider _dataProvider;
        private readonly IGameRetrieverService _drawStore;

        public GameTester(IGameTestExecutor testExecutor, IGameTestDataProvider dataProvider, IGameRetrieverService drawStore)
        {
            _testExecutor = testExecutor;
            _dataProvider = dataProvider;
            _drawStore = drawStore;
        }

        public void BeginTesting()
        {
            var gameToPick = "deer's journey v2";
            var games = _drawStore.ListGames();
            var game = games.FirstOrDefault(a => a.GameName.ToLower() == gameToPick);

            if (game == null)
                throw new Exception($"No game found with name {gameToPick}");

            var data = _dataProvider.ProvideDataForGameTesting(game.GameName);

            _testExecutor.RunTestAsync(game, DateTime.Now.AddMinutes(3), 0, data);

            _dataProvider.SaveDataForGame(data, game.GameName);
        }
    }
}
