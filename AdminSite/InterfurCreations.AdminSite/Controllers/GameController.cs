using BotAdminSite.Models;
using InterfurCreations.AdminSite.Models;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotAdminSite.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly IConfigurationService _configService;
        private readonly IGoogleDriveService _driveService;
        private readonly IGameRetrieverService _gameStore;
        private readonly IPlayerDatabaseController _playerController;
        private readonly IStatisticsService _statisticsService;

        public GameController(IConfigurationService configService, IGoogleDriveService driveService, IGameRetrieverService drawStore, IPlayerDatabaseController playerController, IStatisticsService statisticsService)
        {
            _configService = configService;
            _driveService = driveService;
            _gameStore = drawStore;
            _playerController = playerController;
            _statisticsService = statisticsService;
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

            var allAchievementStats = _statisticsService.ListAchievements();
            var achievementsForGame = allAchievementStats.Where(a => a.GameName == game.GameName);

            var vm = new ViewModelGameDetails
            {
                GameName = game.GameName,
                OptionCount = game.Stats.optionsCount,
                StateCount = game.Stats.states.Count,
                WordCount = game.Stats.wordCount,
                Category = game.Metadata.Category,
                Description = game.Metadata.Description,
                Achievements = new List<AchievementItemViewModel>(),
                Id = gameId
            };

            var totalPlayers = _statisticsService.CountTotalPlayers();

            foreach(var achievement in game.Metadata.Achievements)
            {
                var achievStats = achievementsForGame.FirstOrDefault(a => a.AchievementName == achievement.Name);
                var model = new AchievementItemViewModel();
                model.Description = achievement.Description;
                model.Name = achievement.Name;

                if(achievStats != null)
                {
                    model.NumberOfPlayersAchieved = achievStats.TotalPlayed;
                    model.PercentOfPlayersAchieved =  ((achievStats.TotalPlayed / (double)totalPlayers) * 100).ToString("#.##");
                }
                vm.Achievements.Add(model);
            }
            vm.Achievements = vm.Achievements.OrderByDescending(a => a.NumberOfPlayersAchieved).ToList();

            return View(vm);
        }

        public ActionResult Test(string gameId, int minutesToRunFor)
        {
            var vm = new ViewModelTestResult();
            return View();
        }
    }
}