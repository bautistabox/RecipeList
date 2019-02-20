using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeList.Authentication;

namespace RecipeList.Home
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var sessionUName = HttpContext.Session.GetString("_Username");
            if (sessionUName == null)
            {
                return View();
            }

            return RedirectToAction("Profile", "Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}