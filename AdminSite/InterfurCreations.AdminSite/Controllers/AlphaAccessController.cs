using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InterfurCreations.AdminSite.Core;
using InterfurCreations.AdminSite.Models;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InterfurCreations.AdminSite.Controllers
{
    public class AlphaAccessController : Controller
    {
        private readonly IAccessTokenService _accessTokenService;
        public AlphaAccessController(IAccessTokenService accessTokenService)
        {
            _accessTokenService = accessTokenService;
        }

        public IActionResult List()
        {
            var dbTokens = _accessTokenService.ListTokens();
            var itemModels = dbTokens.Select(a => new AlphaAccessItemModel
            {
                AccessCode = a.Token,
                LastActivated = a.LastActivated,
                PlayerName = a.Player?.Name,
                PlayerPlatform = PlayerPlatformResolver.ResolvePlatformFromPlyer(a.Player).ToString(),
                HoursAllowed = a.HoursForRefresh,
                Id = a.Id
            }).ToList();
            return View(new ViewModelAlphaAccessList { AccessDescriptors = itemModels });
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Delete(int id)
        {
            _accessTokenService.DeleteToken(id);
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult CreateAccessToken(AccessTokenCreateModel tokenCreate)
        {
            if(string.IsNullOrEmpty(tokenCreate.TokenType))
                throw new Exception("Invalid TokenType");
            if(tokenCreate.HoursAllowed < 1)
                throw new Exception("Cannot have Hours Allowed less than 1");
            _accessTokenService.CreateToken(tokenCreate.HoursAllowed, tokenCreate.TokenType);

            return RedirectToAction("List");
        }
    }
}