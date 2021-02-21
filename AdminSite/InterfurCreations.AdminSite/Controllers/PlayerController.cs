using BotAdminSite.Models;
using InterfurCreations.AdminSite.Core;
using InterfurCreations.AdminSite.Core.Interfaces;
using InterfurCreations.AdminSite.Models;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BotAdminSite.Controllers
{
    [Authorize]
    public class PlayerController : Controller
    {
        private readonly IPlayerDatabaseController _playerController;
        private readonly IActionResolver _actionResolver;

        public PlayerController(IActionResolver actionResolver, IPlayerDatabaseController playerController)
        {
            _playerController = playerController;
            _actionResolver = actionResolver;
        }

        public ActionResult List(string playerPlatformFilter = "", string playerNameFilter = "", int pageNumber = 1, int pageSize = 25)
        {
            ViewBag.Message = "Players";

            var model = new ViewModelPlayerList();
            model.PageSize = pageSize;

            if (pageNumber < 0)
                model.PageNumber = 0;

            (List<PlayerListModel> players, int count) result = (null, 0);
            if (Enum.TryParse<PlatformType>(playerPlatformFilter, out var platformType))
            {
                result = _playerController.ListPlayers(platformType, playerNameFilter, pageNumber - 1, pageSize);
            } else
            {
                result = _playerController.ListPlayers(PlatformType.NONE, playerNameFilter, pageNumber - 1, pageSize);
            }

            model.Players = new List<ViewModelPlayer>();
            result.players.ForEach(a =>
            {
                var vmP = new ViewModelPlayer();
                vmP.name = a.Name;
                if (a.LastAction != null) {
                    vmP.actionCount = a.ActionCount;
                    var lastAction = a.LastAction;
                    if(lastAction != null)
                        vmP.lastAction = lastAction.Time;
                } else
                {
                    vmP.actionCount = 0;
                    vmP.lastAction = default(DateTime);
                }
                vmP.id = a.Id;
                vmP.platform = PlayerPlatformResolver.ResolvePlatformFromPlayer(a).ToString();
                model.Players = model.Players;
                model.Players.Add(vmP);
            });


            model.PageNumber = pageNumber;
            model.PlayerNameFilter = playerNameFilter;
            model.PlayerPlatformFilter = playerPlatformFilter;
            model.NumberOfPages = (result.count - 1) / pageSize;
            model.TotalPlayersCount = result.count;

            if (model.Players.Count == 0 && model.PageNumber > 1)
                return List(playerPlatformFilter, playerNameFilter, model.NumberOfPages);

            return View(model);
        }

        public IActionResult Details(string playerId)
        {
            if (playerId == null) return RedirectToAction("List");
            var player = _playerController.GetPlayerById(playerId);
            var actions = _playerController.GetPlayerActions(playerId);
            var saves = _playerController.GetPlayerGameSaves(playerId);
            var model = new PlayerDetailsModel()
            {
                actionCount = player.Actions.Count,
                name = player.Name,
                platform = PlayerPlatformResolver.ResolvePlatformFromPlayer(player).ToString(),
                recentActions = new List<PlayerActionModel>(),
                gameSaves = new List<GameSaveItemModel>(),
                id = player.PlayerId
            };

            var lastAction = actions.OrderByDescending(b => b.Time).FirstOrDefault();
            if (lastAction != null)
            {
                model.lastAction = lastAction.Time;
                var recentActions = actions.OrderByDescending(b => b.Time);
                foreach(var action in recentActions)
                {
                    var resolved = _actionResolver.ResolveAction(action, TimeSpan.FromMinutes(10));
                    if(resolved != null)
                        model.recentActions.Add(new PlayerActionModel
                        {
                            currentStateText = resolved.CurrentStateText,
                            gameName = action.GameName,
                            lastOptionText = resolved.StateOptionTaken
                        });
                }
            }

            model.gameSaves = saves.Select(a => new GameSaveItemModel
            {
                gameName = a.PlayerGameSave.GameName, dateCreated = a.CreatedDate, saveId = a.PlayerGameSaveId
            }).ToList();
            return View(model);
        }
    }
}