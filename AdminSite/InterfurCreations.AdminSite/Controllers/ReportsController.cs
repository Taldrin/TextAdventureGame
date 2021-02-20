using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotAdminSite.Models;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterfurCreations.AdminSite.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IReportsService _reportsService;

        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        public IActionResult Details()
        {
            var vm = new ViewModelReports();
            vm.ActionsByGame = _reportsService.CountActionsByGame(DateTime.Now.Subtract(TimeSpan.FromHours(24)), DateTime.Now);
            vm.ActionsCount = vm.ActionsByGame.Sum(a => a.Value);
            vm.PlayersCount = _reportsService.CountPlayers(DateTime.Now.Subtract(TimeSpan.FromHours(24)), DateTime.Now);
            vm.TotalActions = _reportsService.CountTotalActions();
            vm.TotalPlayers = _reportsService.CountTotalPlayers();
            vm.TotalGameSaves = _reportsService.CountTotalGameSaves();
            return View(vm);
        }
    }
}
