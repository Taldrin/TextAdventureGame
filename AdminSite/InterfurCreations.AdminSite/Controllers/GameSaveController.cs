using InterfurCreations.AdminSite.Models;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace InterfurCreations.AdminSite.Controllers
{
    [Authorize]
    public class GameSaveController : Controller
    {
        private readonly IPlayerDatabaseController _playerController;

        public GameSaveController(IPlayerDatabaseController playerController)
        {
            _playerController = playerController;
        }

        public IActionResult Details(int saveId, string playerId)
        {
            var gameSave = _playerController.GetSaveById(saveId, playerId);

            var model = new GameSaveModel
            {
                dateCreated = gameSave.CreatedDate,
                gameName = gameSave.PlayerGameSave.GameName,
                saveData = gameSave.PlayerGameSave.GameSaveData.Select(a => (a.Name, a.Value)).ToList(),
                saveId = gameSave.PlayerGameSaveId
            };

            return View(model);
        }
    }
}