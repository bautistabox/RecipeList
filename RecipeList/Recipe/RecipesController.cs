using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;
using RecipeList.Authentication;
using RecipeList.Comment;
using RecipeList.Recipes;

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

        [HttpPost]
        [Route("/recipes/save-favorite")]
        public IActionResult SaveFavorite(int rId)
        {
            var uId = HttpContext.Session.GetInt32("_Userid");
            var savedRecipe = new SavedRecipe
            {
                UserId = uId.Value,
                RecipeId = rId,
                SavedAt = DateTime.Now
            };
            _db.SavedRecipes.Add(savedRecipe);
            _db.SaveChanges();
            return RedirectToAction("Page", new {recipeId = rId});
        }

        [HttpPost]
        [Route("/recipes/delete-favorite")]
        public IActionResult DeleteFavorite(int rId)
        {
            var uId = HttpContext.Session.GetInt32("_Userid");
            var savedRecipe = _db.SavedRecipes.FirstOrDefault(sr => sr.RecipeId == rId && sr.UserId == uId);
            if (savedRecipe != null)
            {
                _db.SavedRecipes.Remove(savedRecipe);
                _db.SaveChanges();
            }
            return RedirectToAction("Page", new {recipeId = rId});
        }
        
        [HttpGet]
        public IActionResult Search(RecipeSearchInputModel model)
        {
            // Look for recipes where the search term is in the name
            var dbRecipes = _db.Recipes.Where(r => r.Name.Contains(model.SearchQuery)).ToList();

            // LOOKING FOR RECIPES THAT USE THE SEARCH TERM AS AN INGREDIENT
            // get id of this ingredient
            var dbIngredients = _db.Ingredients.Where(i => i.Name.Contains(model.SearchQuery)).ToList();

            // use the id to select recipe_ingredients
            var dbRecipeIngredients = new List<RecipeIngredients>();
            foreach (var ingredient in dbIngredients)
            {
                var temp = _db.RecipeIngredients.Where(ri => ri.IngredientId == ingredient.Id).ToList();
                foreach (var tmp in temp)
                {
                    dbRecipeIngredients.Add(tmp);
                }
            }

            // use the recipe_ingredients recipe_id to select more recipes that include that ingredient
            var recipeIds = new List<int>();
            foreach (var recipeIngredient in dbRecipeIngredients)
            {
                recipeIds.Add(recipeIngredient.RecipeId);
            }

            // add them to the recipes that use the search term in their name
            foreach (var id in recipeIds)
            {
                var flag = false;
                foreach (var recipe in dbRecipes)
                {
                    if (recipe.Id == id)
                    {
                        flag = true;
                    }
                }

                if (!flag)
                {
                    dbRecipes.Add(_db.Recipes.FirstOrDefault(r => r.Id == id));
                }
            }

            // create a recipe item for each recipe
            var recipeItems = new List<RecipeItems>();
            foreach (var recipe in dbRecipes)
            {
                var recipeOwner = _db.Users.FirstOrDefault(u => u.Id == recipe.UploaderId);
                if (recipeOwner == null)
                {
                    return View("Index");
                }

                var recipeItem = new RecipeItems
                {
                    RecipeId = recipe.Id,
                    RecipeOwner = recipeOwner.DisplayName,
                    RecipeOwnerId = recipe.UploaderId,
                    RecipeName = recipe.Name,
                    RecipeDescription = recipe.Description,
                    RecipeInstruction = recipe.Instruction,
                    RecipePhoto = recipe.Photo,
                    RecipeCreatedDate = recipe.CreatedAt,
                    RecipeUpdatedDate = recipe.UpdatedAt,
                    RecipeRating = _db.RecipeRatings.Where(rr => rr.RecipeId == recipe.Id).Select(rr => rr.Rating)
                        .FirstOrDefault()
                };

                if (recipeItem.RecipePhoto != null)
                {
                    recipeItem.RecipePhoto64 = Convert.ToBase64String(recipeItem.RecipePhoto);
                }

                // this is a shortened version of the recipe description for the preview card
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
            ViewData["currentSearch"] = model.SearchQuery;

            return View("Index", recipeItems);
        }

        [HttpGet]
        [Route("recipes/user/{userId}")]
        public IActionResult UserRecipes(int userId)
        {
            // loads up all the recipes created/saved by a specific user
            var dbUser = _db.Users.FirstOrDefault(u => u.Id == userId);
            var dbRecipes = _db.Recipes.Where(r => r.UploaderId == userId).ToList();
            var dbSavedRecipeObjs = _db.SavedRecipes.Where(sr => sr.UserId == userId).ToList();
            var dbSavedRecipeIds = new List<int>(); 
            foreach (var savedRecipe in dbSavedRecipeObjs)
            {
                dbSavedRecipeIds.Add(savedRecipe.RecipeId);
            }

            var dbSavedRecipes = _db.Recipes.Where(r => dbSavedRecipeIds.Contains(r.Id)).ToList();
            dbRecipes = dbSavedRecipes.Concat(dbRecipes).ToList();
            var recipeItems = new List<RecipeItems>();
            foreach (var recipe in dbRecipes)
            {
                var recipeOwner = _db.Users.FirstOrDefault(u => u.Id == recipe.UploaderId);
                if (recipeOwner == null)
                {
                    return View("Index");
                }

                var recipeItem = new RecipeItems
                {
                    RecipeId = recipe.Id,
                    RecipeOwner = recipeOwner.DisplayName,
                    RecipeOwnerId = recipe.UploaderId,
                    RecipeName = recipe.Name,
                    RecipeDescription = recipe.Description,
                    RecipeInstruction = recipe.Instruction,
                    RecipePhoto = recipe.Photo,
                    RecipeCreatedDate = recipe.CreatedAt,
                    RecipeUpdatedDate = recipe.UpdatedAt,
                    RecipeRating = _db.RecipeRatings.Where(rr => rr.RecipeId == recipe.Id).Select(rr => rr.Rating)
                        .FirstOrDefault()
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
            if (dbUser != null)
            {
                ViewData["currentSearch"] = dbUser.DisplayName + "'s Created/Favorited Recipes";
            }
            
            return View("Index", recipeItems);
        }

        [HttpGet]
        [Route("recipes/search/category/{categoryId}")]
        public IActionResult SearchCategories(int categoryId)
        {
            if (categoryId == 0)
            {
                return View("Index");
            }

            // controller action method handles searching by category
            var dbRecipes = _db.Recipes.Where(r => r.CategoryId == categoryId);

            var recipeItems = new List<RecipeItems>();
            foreach (var recipe in dbRecipes)
            {
                var recipeOwner = _db.Users.FirstOrDefault(u => u.Id == recipe.UploaderId);
                if (recipeOwner == null)
                {
                    return View("Index");
                }

                var recipeItem = new RecipeItems
                {
                    RecipeId = recipe.Id,
                    RecipeOwner = recipeOwner.DisplayName,
                    RecipeOwnerId = recipe.UploaderId,
                    RecipeName = recipe.Name,
                    RecipeDescription = recipe.Description,
                    RecipeInstruction = recipe.Instruction,
                    RecipePhoto = recipe.Photo,
                    RecipeCreatedDate = recipe.CreatedAt,
                    RecipeUpdatedDate = recipe.UpdatedAt,
                    RecipeRating = _db.RecipeRatings.Where(rr => rr.RecipeId == recipe.Id).Select(rr => rr.Rating)
                        .FirstOrDefault()
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
            ViewData["currentSearch"] =
                _db.Categories.Where(c => c.Id == categoryId).Select(c => c.Name).FirstOrDefault();

            return View("Index", recipeItems);
        }

        [HttpGet]
        public IActionResult Index()
        {
            // this controller action method takes 12 random recipes and uses them for the front page

            var model = _db.Recipes
                .Select(r => new RecipeItems
                {
                    RecipeId = r.Id,
                    RecipeOwner = _db.Users.Where(u => u.Id == r.UploaderId).Select(u => u.DisplayName).First(),
                    RecipeOwnerId = r.UploaderId,
                    RecipeName = r.Name,
                    RecipeDescription = r.Description,
                    RecipeInstruction = r.Instruction,
                    RecipePhoto = r.Photo,
                    RecipeCreatedDate = r.CreatedAt,
                    RecipeUpdatedDate = r.UpdatedAt,
                    RecipeRating = _db.RecipeRatings.Where(rr => rr.RecipeId == r.Id).Select(rr => rr.Rating)
                        .FirstOrDefault()
                }).OrderBy(x => Guid.NewGuid()).Take(12).ToList();

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
            var sessionUId = HttpContext.Session.GetInt32("_Userid");
            var user = _db.Users.FirstOrDefault(u => u.Id == sessionUId);

            // this block will grab the recipe items, only this user has entered before
            if (user != null)
            {
                var userRecipesIds = _db.Recipes.Where(r => r.UploaderId == user.Id).Select(r => r.Id).ToList();
                var recipeIngredientsIds =
                    _db.RecipeIngredients.Where(ri => userRecipesIds.Contains(ri.RecipeId))
                        .Select(ri => ri.IngredientId).ToList();
                var ingredients = _db.Ingredients.Where(i => recipeIngredientsIds.Contains(i.Id)).ToList();
                ViewData["ingredients"] = ingredients;
            }

            var dbCategories = _db.Categories.OrderBy(s => s.Name).ToList();
            ViewData["categories"] = dbCategories;
            return View();
        }

        [HttpGet]
        [Route("recipes/edit/{recipeId}")]
        public IActionResult Edit(int recipeId)
        {
            var sessionUId = HttpContext.Session.GetInt32("_Userid");
            var user = _db.Users.FirstOrDefault(u => u.Id == sessionUId);
            // this block will grab the recipe items, only this user has entered before
            if (user != null)
            {
                var userRecipesIds = _db.Recipes.Where(r => r.UploaderId == user.Id).Select(r => r.Id).ToList();
                var recipeIngredientsIds =
                    _db.RecipeIngredients.Where(ri => userRecipesIds.Contains(ri.RecipeId))
                        .Select(ri => ri.IngredientId).ToList();
                var viewIngredients = _db.Ingredients.Where(i => recipeIngredientsIds.Contains(i.Id)).ToList();
                ViewData["ingredients"] = viewIngredients;
            }

            // data to populate edit form with old values
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
            var ingIds = new List<int>();

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

            // formatting the recipe ingredient as
            // "[amount] [unit of measure] of [ingredient]" (1 cup of flour) or
            // "[amount] [ingredient]" (1 chicken breast) or
            // "[ingredient]" (basil)
            for (var i = 0; i < recipeInfo.Ingredients.Count; i++)
            {
                var fullIngredient = "";
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
            if (!ModelState.IsValid || model.Ingredients == null)
            {
                return RedirectToAction("Edit", new {recipeId = model.RecipeId});
            }

            var category = _db.Categories.SingleOrDefault(c => c.Name == model.Category);
            int categoryId;

            var dbRecipe = _db.Recipes.SingleOrDefault(r => r.Id == model.RecipeId);
            if (dbRecipe == null)
            {
                return RedirectToAction("Index");
            }

            // checking if the user specified a category
            if (category == null)
            {
                categoryId = dbRecipe.CategoryId;
            }
            else
            {
                categoryId = category.Id;
            }

            // updating selected recipe
            dbRecipe.Name = model.Name;
            dbRecipe.Description = model.Description;
            dbRecipe.Instruction = model.Instruction;
            dbRecipe.CategoryId = categoryId;
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

            // removing ingredients have been removed
            var deleteIngredients =
                from recipeIngredients in _db.RecipeIngredients
                where recipeIngredients.RecipeId == model.RecipeId
                select recipeIngredients;
            foreach (var ingredient in deleteIngredients)
            {
                _db.RecipeIngredients.Remove(ingredient);
            }

            _db.SaveChanges();

            // adding back ingredients that are still there, checking for duplicates
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

                var dupe = _db.RecipeIngredients.FirstOrDefault(ri =>
                    ri.RecipeId == dbRecipe.Id && ri.IngredientId == ingredient.Id);
                if (dupe == null)
                {
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
            }

            return RedirectToAction("Page", new {recipeId = dbRecipe.Id});
        }

        [HttpPost]
        [Authorize]
        public IActionResult Process(RecipeInputModel model)
        {
            var sessionUId = HttpContext.Session.GetInt32("_Userid");
            // controller action method to process new recipes
            if (!ModelState.IsValid || model.Ingredients == null)
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == sessionUId);

                // this block will grab the recipe items, only this user has entered before
                if (user != null)
                {
                    var userRecipesIds = _db.Recipes.Where(r => r.UploaderId == user.Id).Select(r => r.Id).ToList();
                    var recipeIngredientsIds =
                        _db.RecipeIngredients.Where(ri => userRecipesIds.Contains(ri.RecipeId))
                            .Select(ri => ri.IngredientId).ToList();
                    var ingredients = _db.Ingredients.Where(i => recipeIngredientsIds.Contains(i.Id)).ToList();
                    ViewData["ingredients"] = ingredients;
                }

                var dbCategories = _db.Categories.OrderBy(s => s.Name).ToList();
                ViewData["categories"] = dbCategories;
                if (model.Ingredients == null)
                {
                   ModelState.AddModelError("ingDiv", "Ingredients are required");
                }

                return View("New");
            }

            var catId = _db.Categories.First(u => u.Name == model.Category).Id;
            var recipe = new Recipe();

            if (model.Photo != null) // user provided photo
            {
                var size = model.Photo.Length;
                var stream = new MemoryStream();
                if (size > 0)
                {
                    using (stream)
                    {
                        model.Photo.CopyTo(stream);
                    }
                }

                recipe = new Recipe
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
            }
            else // user did not provide photo
            {
                recipe = new Recipe
                {
                    Name = model.Name,
                    Description = model.Description,
                    Instruction = model.Instruction,
                    CategoryId = catId,
                    UploaderId = sessionUId.Value,
                    CreatedAt = DateTime.Now
                };
                _db.Recipes.Add(recipe);
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

                // checking for duplicate recipe ingredients
                var dupe = _db.RecipeIngredients.FirstOrDefault(ri =>
                    ri.RecipeId == recipe.Id && ri.IngredientId == ingredient.Id);
                if (dupe == null)
                {
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
            }

            return RedirectToAction("Page", new {recipeId = recipe.Id});
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
            if (deleteRecipe != null)
            {
                _db.Recipes.Remove(deleteRecipe);
            }

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

            if (recipeInfo.Recipe == null)
            {
                return View("Index");
            }

            if (recipeInfo.Recipe.Photo != null)
            {
                recipeInfo.Image = Convert.ToBase64String(recipeInfo.Recipe.Photo);
            }

            var recipeIngredients = _db.RecipeIngredients
                .Where(r => r.RecipeId == recipeId)
                .Select(i => new {i.IngredientId, i.Amount, i.AmountType})
                .ToArray();
            var ingIds = new List<int>();

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
                var fullIngredient = "";
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

            var comments = _db.Comments.Where(c => c.RecipeId == recipeInfo.Recipe.Id)
                .OrderByDescending(u => u.CreatedAt).ToList();
            var commentInfoList = new List<CommentInfo>();
            foreach (var comment in comments)
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == comment.UserId);
                if (user != null)
                {
                    var commentInfo = new CommentInfo
                    {
                        CommentObj = comment,
                        Commenter = user.DisplayName
                    };

                    commentInfoList.Add(commentInfo);
                }
            }

            var rating = 0;
            var dbRating = _db.RecipeRatings.Where(r => r.RecipeId == recipeInfo.Recipe.Id).ToList();
            recipeInfo.RatingCount = dbRating.Count;
            if (dbRating.Count > 0)
            {
                // averaging the rating
                foreach (var rate in dbRating)
                {
                    rating += rate.Rating;
                }

                recipeInfo.Rating = rating / recipeInfo.RatingCount;
            }
            else
            {
                recipeInfo.Rating = rating;
            }


            ViewData["CommentInfo"] = commentInfoList;
            var recipeOwner = _db.Users.FirstOrDefault(u => u.Id == recipeInfo.Recipe.UploaderId);
            if (recipeOwner != null)
            {
                ViewData["RecipeOwner"] = recipeOwner.DisplayName;
            }

            var savedRecipe =
                _db.SavedRecipes.FirstOrDefault(sr => sr.RecipeId == recipeId && sr.UserId == recipeInfo.CurrentUser);

            if (savedRecipe != null)
            {
                ViewData["AlreadySaved"] = true;
            }
            else
            {
                ViewData["AlreadySaved"] = false;
            }


            return View(recipeInfo);
        }

        [HttpPost]
        [Route("recipes/submit-rating")]
        public IActionResult SubmitRating(int rating, int currentUser, int rId)
        {
            var dbRating = _db.RecipeRatings.FirstOrDefault(rr => rr.UserId == currentUser && rr.RecipeId == rId);
            if (dbRating == null)
            {
                var newRating = new RecipeRating
                {
                    Rating = rating,
                    RecipeId = rId,
                    UserId = currentUser
                };

                _db.RecipeRatings.Add(newRating);
            }
            else
            {
                dbRating.Rating = rating;
            }

            _db.SaveChanges();

            return RedirectToAction("Page", new {recipeId = rId});
        }
    }
}