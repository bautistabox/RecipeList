using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using RecipeList.Accounts;
using RecipeList.Authentication;
using RecipeList.Ingredient;
using RecipeList.Recipe;

namespace RecipeList.Recipes
{
    [Authorize]
    public class RecipesController : Controller
    {
        private readonly RecipesDbContext _db;

        public RecipesController(RecipesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var sessionUId = HttpContext.Session.GetInt32("_Userid");
            var sessionUName = HttpContext.Session.GetString("_Username");

            var model = _db.Recipes
                .Where(u => u.UploaderId == sessionUId)
                .Select(u => new RecipeItems
                {
                    RecipeId = u.Id,
                    RecipeOwner = sessionUName,
                    RecipeOwnderId = sessionUId.Value,
                    RecipeName = u.Name,
                    RecipeDescription = u.Description,
                    RecipeInstruction = u.Instruction,
                    RecipePhoto = u.Photo,
                    RecipeCreatedDate = u.CreatedAt,
                    RecipeUpdatedDate = u.UpdatedAt
                }).ToList();

            foreach (var mod in model)
            {
                if (mod.RecipePhoto != null)
                {
                    mod.RecipePhoto64 = Convert.ToBase64String(mod.RecipePhoto);
                }

                if (mod.RecipeDescription.Length > 80)
                {
                    mod.RecipeDescShort = mod.RecipeDescription.Substring(0, 80) + "...";
                }
                else
                {
                    mod.RecipeDescShort = mod.RecipeDescription;
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult New()
        {
            var dbIngredients = _db.Ingredients.ToList();
            var dbCategories = _db.Categories.OrderBy(s => s.Name).ToList();
            ViewData["ingredients"] = dbIngredients;
            ViewData["categories"] = dbCategories;
            return View();
        }

        [HttpGet]
        [Route("recipes/edit/{recipeId}")]
        public IActionResult Edit(int recipeId)
        {
            var dbIngredients = _db.Ingredients.ToList();
            ViewData["ingredients"] = dbIngredients;
            
            
            var recipeInfo = new RecipeInfo();
            recipeInfo.Categories = _db.Categories.OrderBy(s => s.Name).ToList();
            recipeInfo.RecipeIngredients = new List<RecipeIngredients>();
            recipeInfo.Ingredients = new List<Ingredient.Ingredient>();
            recipeInfo.Category = new Category();
            recipeInfo.IngredientsList = new List<string>();
            recipeInfo.Recipe = _db.Recipes.FirstOrDefault(r => r.Id == recipeId);
            
            var recipeIngredients = _db.RecipeIngredients
                .Where(r => r.RecipeId == recipeId)
                .Select(i => new {i.IngredientId, i.Amount, i.AmountType})
                .ToArray();
            List<int> ingIds = new List<int>();

            for (var i = 0; i < recipeIngredients.Length; i++)
            {
                recipeInfo.RecipeIngredients.Add(
                    new RecipeIngredients
                    {
                        IngredientId = recipeIngredients[i].IngredientId,
                        Amount = recipeIngredients[i].Amount,
                        AmountType = recipeIngredients[i].AmountType
                    });
                ingIds.Add(recipeIngredients[i].IngredientId);
            }

            recipeInfo.Category = _db.Categories.SingleOrDefault(c => c.Id == recipeInfo.Recipe.CategoryId);

            var ingredients = _db.Ingredients
                .Where(i => ingIds.Contains(i.Id));

            foreach (var ingredient in ingredients)
            {
                recipeInfo.Ingredients.Add(
                    new Ingredient.Ingredient
                    {
                        Id = ingredient.Id,
                        Name = ingredient.Name
                    });
            }

            for (var i = 0; i < recipeInfo.Ingredients.Count; i++)
            {
                string fullIngredient = "";
                if (recipeInfo.RecipeIngredients[i].Amount != "N/A")
                {
                    fullIngredient = recipeInfo.RecipeIngredients[i].Amount + " ";
                    if (recipeInfo.RecipeIngredients[i].AmountType != "N/A")
                    {
                        fullIngredient = fullIngredient + recipeInfo.RecipeIngredients[i].AmountType + " of ";
                    }
                }

                fullIngredient = fullIngredient + recipeInfo.Ingredients[i].Name;
                recipeInfo.IngredientsList.Add(fullIngredient);
            }
            
            return View(recipeInfo);
        }

        [HttpPost]
        public IActionResult Process(RecipeInputModel model)
        {
            var sessionUId = HttpContext.Session.GetInt32("_Userid");
            var sessionUName = HttpContext.Session.GetString("_Username");

            var catId = _db.Categories.First(u => u.Name == model.Category).Id;
            var size = model.Photo.Length;
            var filePath = Path.GetTempFileName();
            var stream = new MemoryStream();
            if (size > 0)
            {
                using (stream)
                {
                    model.Photo.CopyTo(stream);
                }
            }

            var recipe = new Recipe
            {
                Name = model.Name,
                Description = model.Description,
                Instruction = model.Instruction,
                CategoryId = catId,
                Photo = stream.ToArray(),
                UploaderId = sessionUId.Value,
                CreatedAt = DateTime.Now
            };
            _db.Recipes.Add(recipe);
            _db.SaveChanges();

            foreach (var ingredientItem in model.Ingredients)
            {
                var ingredient = new Ingredient.Ingredient();
                var dbIngredient = _db.Ingredients.SingleOrDefault(i => i.Name == ingredientItem.Ingredient);
                if (dbIngredient == null)
                {
                    ingredient = new Ingredient.Ingredient
                    {
                        Name = ingredientItem.Ingredient,
                        CreatedAt = DateTime.Now
                    };
                    _db.Ingredients.Add(ingredient);
                    _db.SaveChanges();
                }
                else
                {
                    ingredient = dbIngredient;
                }

                var recipeIngredient = new RecipeIngredients
                {
                    IngredientId = ingredient.Id,
                    RecipeId = recipe.Id,
                    Amount = ingredientItem.Amount,
                    AmountType = ingredientItem.AmountType
                };
                _db.RecipeIngredients.Add(recipeIngredient);
                _db.SaveChanges();
            }

            return View();
        }

        [HttpGet]
        [Route("recipes/page/{recipeId}")]
        public IActionResult Page(int recipeId)
        {
            var recipeInfo = new RecipeInfo();
            recipeInfo.RecipeIngredients = new List<RecipeIngredients>();
            recipeInfo.Ingredients = new List<Ingredient.Ingredient>();
            recipeInfo.Category = new Category();
            recipeInfo.IngredientsList = new List<string>();
            recipeInfo.Recipe = _db.Recipes.FirstOrDefault(r => r.Id == recipeId);

            if (recipeInfo.Recipe.Photo != null)
            {
                recipeInfo.Image = Convert.ToBase64String(recipeInfo.Recipe.Photo);
            }

            var recipeIngredients = _db.RecipeIngredients
                .Where(r => r.RecipeId == recipeId)
                .Select(i => new {i.IngredientId, i.Amount, i.AmountType})
                .ToArray();
            List<int> ingIds = new List<int>();

            for (var i = 0; i < recipeIngredients.Length; i++)
            {
                recipeInfo.RecipeIngredients.Add(
                    new RecipeIngredients
                    {
                        IngredientId = recipeIngredients[i].IngredientId,
                        Amount = recipeIngredients[i].Amount,
                        AmountType = recipeIngredients[i].AmountType
                    });
                ingIds.Add(recipeIngredients[i].IngredientId);
            }

            recipeInfo.Category = _db.Categories.SingleOrDefault(c => c.Id == recipeInfo.Recipe.CategoryId);

            var ingredients = _db.Ingredients
                .Where(i => ingIds.Contains(i.Id));

            foreach (var ingredient in ingredients)
            {
                recipeInfo.Ingredients.Add(
                    new Ingredient.Ingredient
                    {
                        Id = ingredient.Id,
                        Name = ingredient.Name
                    });
            }

            for (var i = 0; i < recipeInfo.Ingredients.Count; i++)
            {
                string fullIngredient = "";
                if (recipeInfo.RecipeIngredients[i].Amount != "N/A")
                {
                    fullIngredient = recipeInfo.RecipeIngredients[i].Amount + " ";
                    if (recipeInfo.RecipeIngredients[i].AmountType != "N/A")
                    {
                        fullIngredient = fullIngredient + recipeInfo.RecipeIngredients[i].AmountType + " of ";
                    }
                }

                fullIngredient = fullIngredient + recipeInfo.Ingredients[i].Name;
                recipeInfo.IngredientsList.Add(fullIngredient);
            }

            return View(recipeInfo);
        }
    }
}