using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
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
            var user = HttpContext.Session.GetString("_Username");
            if (!user.Equals("admin"))
            {
                return RedirectToAction("Login", "Account");
            }

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
            var currentCategories = new List<string>();
            foreach (var category in _db.Categories)
            {
                currentCategories.Add(category.Name);
            }

            foreach (var categoryItem in categoriesList.Categories)
            {
                if (!currentCategories.Contains(categoryItem))
                {
                    var cat = new Category
                    {
                        Name = categoryItem
                    };

                    _db.Categories.Add(cat);
                }
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