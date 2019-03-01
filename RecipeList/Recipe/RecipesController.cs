using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;
using RecipeList.Authentication;
namespace RecipeList.Recipe
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
        public IActionResult Search(RecipeSearchInputModel model)
        {
            var dbRecipes = _db.Recipes.ToList();

            Console.WriteLine(model.SearchQuery);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("recipes/search/category/{categoryId}")]
        public IActionResult SearchCategories(int categoryId)
        {
            if (categoryId == 0)
            {
                return View("Index");
            }
            
            var dbRecipes = _db.Recipes.Where(r => r.CategoryId == categoryId);

            var recipeItems = new List<RecipeItems>();
            foreach (var recipe in dbRecipes)
            {
                var recipeOwner = _db.Users.FirstOrDefault(u => u.Id == recipe.UploaderId);
                
                var recipeItem = new RecipeItems
                {
                    RecipeId = recipe.Id,
                    RecipeOwner = recipeOwner.DisplayName,
                    RecipeOwnderId = recipe.UploaderId,
                    RecipeName = recipe.Name,
                    RecipeDescription = recipe.Description,
                    RecipeInstruction = recipe.Instruction,
                    RecipePhoto = recipe.Photo,
                    RecipeCreatedDate = recipe.CreatedAt,
                    RecipeUpdatedDate = recipe.UpdatedAt
                };

                if (recipeItem.RecipePhoto != null)
                {
                    recipeItem.RecipePhoto64 = Convert.ToBase64String(recipeItem.RecipePhoto);
                }
                if (recipeItem.RecipeDescription.Length > 68)
                {
                    recipeItem.RecipeDescShort = recipeItem.RecipeDescription.Substring(0, 68) + "...";
                }
                else
                {
                    recipeItem.RecipeDescShort = recipeItem.RecipeDescription;
                }
                
                recipeItems.Add(recipeItem);
            }
            
            var dbCategories = _db.Categories.OrderBy(s => Guid.NewGuid()).Take(8).ToList();
            var allDbCategories = _db.Categories.OrderBy(s => s.Name).ToList();
            ViewData["categories"] = dbCategories;
            ViewData["allCategories"] = allDbCategories;

            return View("Index", recipeItems);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var sessionUId = HttpContext.Session.GetInt32("_Userid");
            var user = _db.Users.FirstOrDefault(u => u.Id == sessionUId);

            var model = _db.Recipes
                .Select(u => new RecipeItems
                {
                    RecipeId = u.Id,
                    RecipeOwner = user.DisplayName,
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

                if (mod.RecipeDescription.Length > 68)
                {
                    mod.RecipeDescShort = mod.RecipeDescription.Substring(0, 68) + "...";
                }
                else
                {
                    mod.RecipeDescShort = mod.RecipeDescription;
                }
            }
            
            var dbCategories = _db.Categories.OrderBy(s => Guid.NewGuid()).Take(8).ToList();
            var allDbCategories = _db.Categories.OrderBy(s => s.Name).ToList();
            ViewData["categories"] = dbCategories;
            ViewData["allCategories"] = allDbCategories;
            
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
        public IActionResult Update(RecipeInputModel model)
        {
            var dbRecipe = _db.Recipes.SingleOrDefault(r => r.Id == model.RecipeId);
            dbRecipe.Name = model.Name;
            dbRecipe.Description = model.Description;
            dbRecipe.Instruction = model.Instruction;
            dbRecipe.UpdatedAt = DateTime.Now;
            if (model.Photo != null)
            {
                var stream = new MemoryStream();
                if (model.Photo.Length > 0)
                {
                    using (stream)
                    {
                        model.Photo.CopyTo(stream);
                    }

                    dbRecipe.Photo = stream.ToArray();
                }
            }

            _db.SaveChanges();

            var deleteIngredients =
                from recipeIngredients in _db.RecipeIngredients
                where recipeIngredients.RecipeId == model.RecipeId
                select recipeIngredients;
            foreach (var ingredient in deleteIngredients)
            {
                _db.RecipeIngredients.Remove(ingredient);
            }

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
                    RecipeId = dbRecipe.Id,
                    Amount = ingredientItem.Amount,
                    AmountType = ingredientItem.AmountType
                };
                _db.RecipeIngredients.Add(recipeIngredient);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Process(RecipeInputModel model)
        {
            var sessionUId = HttpContext.Session.GetInt32("_Userid");
            var sessionUName = HttpContext.Session.GetString("_Username");

            var catId = _db.Categories.First(u => u.Name == model.Category).Id;
            var size = model.Photo.Length;
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

        [HttpPost]
        public IActionResult Delete(DeleteItem model)
        {
            var deleteIngredients =
                from recipeIngredients in _db.RecipeIngredients
                where recipeIngredients.RecipeId == model.DeleteRecipeId
                select recipeIngredients;
            foreach (var ingredient in deleteIngredients)
            {
                _db.RecipeIngredients.Remove(ingredient);
            }

            _db.SaveChanges();

            var deleteRecipe = _db.Recipes.FirstOrDefault(d => d.Id == model.DeleteRecipeId);
            _db.Recipes.Remove(deleteRecipe);

            _db.SaveChanges();
            return RedirectToAction("Index");
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
            recipeInfo.CurrentUser = HttpContext.Session.GetInt32("_Userid").Value;

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