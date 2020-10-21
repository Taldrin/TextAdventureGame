using BotAdminSite.Models;
using InterfurCreations.AdminSite.Models;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotAdminSite.Controllers
{
    public class GameController : Controller
    {
        private readonly IConfigurationService _configService;
        private readonly IGoogleDriveService _driveService;
        private readonly IGameStore _gameStore;
        private readonly IPlayerDatabaseController _playerController;

        public GameController(IConfigurationService configService, IGoogleDriveService driveService, IGameStore drawStore, IPlayerDatabaseController playerController)
        {
            _configService = configService;
            _driveService = driveService;
            _gameStore = drawStore;
            _playerController = playerController;
        }

        public async Task<ActionResult> List(CancellationToken cancellationToken)
        {
            var games = _gameStore.ListGames();
            var viewModel = new ViewModelGamesList();
            viewModel.Games = games.Select(a => new GameItem { Name = a.GameName, Id = a.StartState.Id }).ToList();
            return View(viewModel);
        }

        public ActionResult Details(string gameId)
        {
            var games = _gameStore.ListGames();
            var game = games.Find(a => a.StartState.Id == gameId);

            var vm = new ViewModelGameDetails
            {
                GameName = game.GameName,
                OptionCount = game.Stats.optionsCount,
                StateCount = game.Stats.states.Count,
                WordCount = game.Stats.wordCount,
                Achievements = game.Metadata.Achievements,
                Category = game.Metadata.Category,
                Description = game.Metadata.Description,
                Id = gameId
            };
            return View(vm);
        }

        public ActionResult Test(string gameId, int minutesToRunFor)
        {
            var vm = new ViewModelTestResult();
            return View();
        }
    }
}