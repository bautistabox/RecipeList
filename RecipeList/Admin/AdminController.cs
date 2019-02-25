using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RecipeList.Recipes;
using RecipeList.Shopping;

namespace RecipeList.Accounts
{
    public class AdminController : Controller
    {
        private readonly RecipesDbContext _db;

        public AdminController(RecipesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Home()
        {
            var categoriesList = new CategoriesList();

            foreach (var category in _db.Categories)
            {
                categoriesList.Categories.Add(category);
            }

            return View(categoriesList);
        }

        [HttpPost]
        public IActionResult Add(DataInput dataInput)
        {
            var categoriesList = JsonConvert.DeserializeObject<CategoryInputModel>(dataInput.Data);

            foreach (var category in _db.Categories)
            {
                _db.Remove(category);
            }
            
            foreach (var category in categoriesList.Categories)
            {
                var cat = new Category
                {
                    Name = category
                };

                _db.Categories.Add(cat);
            }

            _db.SaveChanges();

            return RedirectToAction("Saved");
        }

        [HttpGet]
        public IActionResult Saved()
        {
            return View();
        }
    }
}