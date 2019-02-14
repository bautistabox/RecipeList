using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace RecipeList.Recipes
{
    public class RecipeController : Controller
    {
        private readonly RecipesDbContext _db;

        public RecipeController(RecipesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Recipes()
        {
            var sessionUName = HttpContext.Session.GetString("_Username");
            if (sessionUName != null)
            {
                return View();
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult New()
        {
            var sessionUName = HttpContext.Session.GetString("_Username");
            if (sessionUName == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var dbIngredients = _db.Ingredients.ToList();

            ViewData["ingredients"] = dbIngredients;

            return View();
        }
    }
}