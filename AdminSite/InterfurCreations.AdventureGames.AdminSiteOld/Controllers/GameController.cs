using BotAdminSite.Models;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BotAdminSite.Controllers
{
    public class GameController : Controller
    {
        private readonly IConfigurationService _configService;
        private readonly IGoogleDriveService _driveService;
        private readonly IGameStore _gameStore;

        public GameController(IConfigurationService configService, IGoogleDriveService driveService, IGameStore drawStore)
        {
            _configService = configService;
            _driveService = driveService;
            _gameStore = drawStore;
        }

        public async Task<ActionResult> List(CancellationToken cancellationToken)
        {
            var games = _gameStore.ListGames();
            var viewModel = new ViewModelGamesList();
            viewModel.Games = games.Select(a => new GameItem { Name = a.GameName, Id = a.startState.Id }).ToList();
            return View(viewModel);
        }
    }
}