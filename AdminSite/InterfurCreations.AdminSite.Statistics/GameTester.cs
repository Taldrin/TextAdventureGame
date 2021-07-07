using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.GameTesting;
using InterfurCreations.AdventureGames.Graph;
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

        public void BeginTesting(string gameName, int minutesToRunFor, int maxActionsPerRun, string startState = null, List<PlayerGameSaveData> startData = null)
        {
            var games = _drawStore.ListGames();

            DrawGame game;

            if(string.IsNullOrEmpty(gameName))
            {
                game = games[new Random().Next(games.Count())];
            }
            else
            {
                var gameToPick = gameName.ToLower();
                game = games.FirstOrDefault(a => a.GameName.ToLower() == gameToPick);
                if (game == null)
                    throw new Exception($"No game found with name {gameToPick}");
            }

            var data = _dataProvider.ProvideDataForGameTesting(game.GameName);

            _testExecutor.RunTestAsync(game, DateTime.Now.AddMinutes(minutesToRunFor), maxActionsPerRun, data, startState, startData);

            _dataProvider.SaveDataForGame(data, game.GameName);
        }
    }
}
