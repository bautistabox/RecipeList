using Microsoft.AspNetCore.Mvc;
using System.Linq;
using RecipeList.Authentication;

namespace RecipeList.Recipes
{
    [Authorize]
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
            return View();
        }

        [HttpGet]
        public IActionResult New()
        {
            var dbIngredients = _db.Ingredients.ToList();
            ViewData["ingredients"] = dbIngredients;
            return View();
        }
    }
}