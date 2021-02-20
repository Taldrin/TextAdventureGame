using Autofac;
using InterfurCreations.AdminSite;
using InterfurCreations.AdminSite.Core.Interfaces;
using InterfurCreations.AdventureGames.Graph;
using Microsoft.AspNetCore.Mvc;

namespace BotAdminSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}
    }
}