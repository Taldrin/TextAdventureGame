using BotAdminSite.Models;
using InterfurCreations.AdminSite.Models;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InterfurCreations.AdminSite.Core;
using InterfurCreations.AdventureGames.Graph;
using Hangfire;
using InterfurCreations.AdminSite.BackgroundTasks.Tasks;
using System;
using InterfurCreations.AdventureGames.Database;

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
        private readonly GameTestingReportCompiler _gameTestReportCompiler;

        public GameController(GameTestingReportCompiler testReportCompiler, IConfigurationService configService, IGoogleDriveService driveService, IGameRetrieverService drawStore, IPlayerDatabaseController playerController, IStatisticsService statisticsService)
        {
            _configService = configService;
            _driveService = driveService;
            _gameStore = drawStore;
            _playerController = playerController;
            _statisticsService = statisticsService;
            _gameTestReportCompiler = testReportCompiler;
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
            var playersPlayed = _statisticsService.GetPlayerCountForGame(game.GameName);

            var vm = new ViewModelGameDetails
            {
                GameName = game.GameName,
                OptionCount = game.Stats.optionsCount,
                StateCount = game.Stats.states.Count,
                WordCount = game.Stats.wordCount,
                PlayersPlayed = playersPlayed.TotalPlayed,
                Category = game.Metadata.Category,
                Description = game.Metadata.Description,
                Achievements = new List<AchievementItemViewModel>(),
                Id = gameId
            };


            foreach(var achievement in game.Metadata.Achievements)
            {
                var achievStats = achievementsForGame.FirstOrDefault(a => a.AchievementName == achievement.Name);
                var model = new AchievementItemViewModel();
                model.Description = achievement.Description;
                model.Name = achievement.Name;

                if(achievStats != null)
                {
                    model.NumberOfPlayersAchieved = achievStats.TotalPlayed;
                    model.PercentOfPlayersAchieved =  ((achievStats.TotalPlayed / (double)playersPlayed.TotalPlayed) * 100).ToString("#.##");
                }
                vm.Achievements.Add(model);
            }
            vm.Achievements = vm.Achievements.OrderByDescending(a => a.NumberOfPlayersAchieved).ToList();

            return View(vm);
        }

        public ActionResult Testing(string gameId)
        {
            var vm = new ViewModelTestResult();
            var games = _gameStore.ListGames();
            var game = games.Find(a => a.StartState.Id == gameId);
            var report = _gameTestReportCompiler.CompileReportForGame(game.GameName);

            vm.GameName = game.GameName;
            vm.GameTestReport = report;
            return View(vm);
        }

        public ActionResult TestDataDelete(string gameName)
        {
            _gameTestReportCompiler.DeleteAllDataForGame(gameName);
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult State(string gameName, string stateId)
        {
            var game = _gameStore.ListGames().Find(a => a.GameName == gameName);
            var state = game.FindStateById(stateId);

            var vm = new ViewModelStateDetails()
            {
                GameName = gameName,
                StateId = stateId,
                StateText = state.StateText,
                StateAttachments = state.StateAttachements.Select(a => a.StateText).ToList(),
                StateOptions = state.StateOptions.Select(a =>
                    new ViewModelOptionDetails
                    {
                        OptionId = a.Id,
                        OptionText = a.StateText,
                        ResultStateId = a.ResultState.Id,
                        ResultStateText = a.ResultState.StateText
                    }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult RunCustomTest(ViewModelTestResult testResult)
        {
            var startData = testResult.CustomTestStartData.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(a => new PlayerGameSaveData { Name = a.Split(':')[0], Value = a.Split(':')[1] }).ToList();
            BackgroundJob.Enqueue<CustomGameTestTask>(a => a.Run(testResult.GameName, testResult.CustomTestMinutesToRunFor, testResult.CustomTestMaxActions, testResult.CustomTestStartState, startData));
            
            for(int i = 1; i < testResult.CustomTestTimesToRun; i++)
            {
                BackgroundJob.Schedule<CustomGameTestTask>(
                    a => a.Run(testResult.GameName, testResult.CustomTestMinutesToRunFor, testResult.CustomTestMaxActions, testResult.CustomTestStartState, startData),
                    TimeSpan.FromMinutes(testResult.CustomTestMinutesToRunFor * (i + 1)));
            }
                
            return RedirectToAction("List");
        }
    }
}