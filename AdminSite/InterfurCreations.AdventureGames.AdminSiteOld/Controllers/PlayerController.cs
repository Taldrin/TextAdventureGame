using BotAdminSite.Models;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BotAdminSite.Controllers
{
    public class PlayerController : Controller
    {
        private readonly IPlayerDatabaseController _playerController;

        public PlayerController(IPlayerDatabaseController playerController)
        {
            _playerController = playerController;
        }

        public ActionResult List()
        {
            ViewBag.Message = "Players";
            var players = _playerController.ListPlayers();
            var model = new ViewModelPlayerList();

            model.TotalActionsCount = players.Sum(a => a.Actions.Count);
            model.Players = new List<ViewModelPlayer>();
            players.ForEach(a =>
            {
                var vmP = new ViewModelPlayer();
                vmP.name = a.Name;
                if (a.Actions != null) {
                    vmP.actionCount = a.Actions.Count;
                    var lastAction = a.Actions.OrderByDescending(b => b.Time).FirstOrDefault();
                    if(lastAction != null)
                        vmP.lastAction = lastAction.Time;
                } else
                {
                    vmP.actionCount = 0;
                    vmP.lastAction = default(DateTime);
                }
                vmP.id = a.PlayerId;
                model.Players = model.Players.OrderByDescending(b => b.lastAction).ToList();
                model.Players.Add(vmP);
            });

            return View(model);
        }
    }
}